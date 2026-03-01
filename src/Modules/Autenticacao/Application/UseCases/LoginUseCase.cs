using Modules.Autenticacao.Application.DTOs;
using Modules.Autenticacao.Domain.Entities;
using Modules.Autenticacao.Domain.Interfaces;
using System.Net;
using System.Security.Cryptography;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace Modules.Autenticacao.Application.UseCases
{
    public class LoginUseCase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        private const int LIMITE_MAXIMO_TENTATIVAS = 3;
        private const int TEMPO_DESBLOQUEIO_MINUTOS = 15;
        private const int VERSAO_TERMOS_ATUAL = 1;

        public LoginUseCase(
            IUsuarioRepository usuarioRepository,
            ITokenService tokenService,
            IEmailService emailService, IConfiguration configuration)
        {
            _usuarioRepository = usuarioRepository;
            _tokenService = tokenService;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<(LoginResponse? response, HttpStatusCode statusCode)> ExecutarAsync(LoginRequest request)
        {
            if (CredenciaisInvalidas(request))
            {
                return (null, HttpStatusCode.BadRequest);
            }

            var usuario = await _usuarioRepository.BuscarPorEmailAsync(request.Email);
            if (usuario is null)
            {
                return (null, HttpStatusCode.Unauthorized);
            }

            var statusUsuario = ValidarEstadoUsuario(usuario);
            if (statusUsuario != HttpStatusCode.OK)
            {
                return (null, statusUsuario);
            }

            if (!SenhaValida(request.Senha, usuario))
            {
                await RegistrarFalhaLoginAsync(usuario);
                return (null, HttpStatusCode.Unauthorized);
            }

            await RegistrarLoginComSucessoAsync(usuario);

            var response = CriarResponse(usuario);

            return (response, HttpStatusCode.OK);
        }

        private static bool CredenciaisInvalidas(LoginRequest request)
        {
            return string.IsNullOrWhiteSpace(request.Email)
                || string.IsNullOrWhiteSpace(request.Senha);
        }

        private static HttpStatusCode ValidarEstadoUsuario(Usuario usuario)
        {
            if (!usuario.EstaAtivo())
            {
                return HttpStatusCode.Forbidden;
            }

            if (usuario.EstaBloqueado())
            {
                return HttpStatusCode.Locked;
            }

            return HttpStatusCode.OK;
        }

        private bool SenhaValida(string senhaInformada, Usuario usuario)
        {
            return BCrypt.Net.BCrypt.Verify(senhaInformada, usuario.SenhaHash);
        }

        private async Task RegistrarFalhaLoginAsync(Usuario usuario)
        {
            usuario.IncrementarTentativasFalhas(
                LIMITE_MAXIMO_TENTATIVAS,
                TEMPO_DESBLOQUEIO_MINUTOS);

            await _usuarioRepository.AtualizarAsync(usuario);
        }

        private async Task RegistrarLoginComSucessoAsync(Usuario usuario)
        {
            usuario.ResetarTentativas();
            usuario.AtualizarUltimoLogin();

            await _usuarioRepository.AtualizarAsync(usuario);
        }

        private LoginResponse CriarResponse(Usuario usuario)
        {
            var token = _tokenService.GerarToken(usuario);

            return new LoginResponse
            {
                Token = token,
                NecessitaAceitarTermos = usuario.NecessitaAceitarTermos(VERSAO_TERMOS_ATUAL),
                Usuario = new UsuarioDto
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    IdPerfil = usuario.IdPerfil
                }
            };
        }

        public async Task<bool> ResetarSenhaAsync(Modules.Autenticacao.Application.DTOs.ResetPasswordRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.NovaSenha))
            {
                return false;
            }

            var usuario = await _usuarioRepository.BuscarPorEmailAsync(request.Email);
            if (usuario is null)
            {
                return false;
            }

            usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.NovaSenha);

            await _usuarioRepository.AtualizarAsync(usuario);

            return true;
        }
        
        public async Task<string?> SolicitarResetAsync(EsqueceuSenhaDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email))
                return null;

            var usuario = await _usuarioRepository.BuscarPorEmailAsync(request.Email);
            if (usuario is null)
            {
                return null;
            }

            var tokenBytes = new byte[32];
            RandomNumberGenerator.Fill(tokenBytes);
            var token = WebEncoders.Base64UrlEncode(tokenBytes);

            usuario.ResetSenhaToken = token;
            usuario.DataExpiraTokenResetSenha = DateTime.UtcNow.AddMinutes(15);

            await _usuarioRepository.AtualizarAsync(usuario);

            try
            {

                var api = _configuration["UrlSettings:FrontendUrl"];
                
                var resetLink = $"{api}/redefinir-senha?token={token}&email={usuario.Email}";
                await _emailService.EnviarEmailResetSenhaAsync(usuario.Email, usuario.Nome, token, resetLink);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao enviar e-mail de reset: {ex.Message}");
            }

            return token;
        }

        public async Task EfetuarResetAsync(ResetSenhaDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Token) || string.IsNullOrWhiteSpace(request.NovaSenha))
                throw new ArgumentException("Dados inválidos para reset de senha");

            var usuario = await _usuarioRepository.BuscarPorEmailAsync(request.Email);
            if (usuario is null)
                throw new InvalidOperationException("Usuário não encontrado");

            if (string.IsNullOrWhiteSpace(usuario.ResetSenhaToken) || usuario.DataExpiraTokenResetSenha == null)
                throw new InvalidOperationException("Token inválido ou não solicitado");

            // Verifica expiração
            if (usuario.DataExpiraTokenResetSenha < DateTime.UtcNow)
            {
                usuario.ResetSenhaToken = null;
                usuario.DataExpiraTokenResetSenha = null;
                await _usuarioRepository.AtualizarAsync(usuario);
                throw new InvalidOperationException("Token expirado");
            }

            var provided = Encoding.UTF8.GetBytes(request.Token);
            var stored = Encoding.UTF8.GetBytes(usuario.ResetSenhaToken);
            if (!CryptographicOperations.FixedTimeEquals(provided, stored))
                throw new InvalidOperationException("Token inválido");

            usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.NovaSenha);
            usuario.ResetSenhaToken = null;
            usuario.DataExpiraTokenResetSenha = null;

            await _usuarioRepository.AtualizarAsync(usuario);
        }
    }
}
