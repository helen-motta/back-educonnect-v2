using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Modules.Autenticacao.Application.DTOs;
using Modules.Autenticacao.Domain.Enums;
using Modules.Autenticacao.Domain.Interfaces;

namespace Modules.Autenticacao.Application.UseCases
{
    public class UsuarioUseCase
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioUseCase(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<List<UsuarioDto>> ObterTodosUsuariosAsync()
        {
            var usuarios = await _usuarioRepository.ObterUsuarios();
            return usuarios.Select(u => new UsuarioDto
            {
                Id = u.Id,
                Nome = u.Nome,
                Email = u.Email,
                IdPerfil = u.IdPerfil,
                Registro = u.Registro
            }).ToList();
        }

        public async Task<UsuarioRequestDto> CriarUsuarioAsync(UsuarioRequestDto usuarioDto)
        {
            var usuario = new Domain.Entities.Usuario
            {
                Nome = usuarioDto.Nome,
                Email = await GerarEmailAsync(usuarioDto.Nome),
                IdPerfil = usuarioDto.IdPerfil,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(GerarSenha()),
                CriadoEm = DateTime.UtcNow,
                Registro = await GerarRegistro(usuarioDto.IdPerfil),
                Cep = usuarioDto.Cep,
                Endereco = usuarioDto.Endereco,
                Numero = usuarioDto.Numero,
                Complemento = usuarioDto.Complemento,
                Bairro = usuarioDto.Bairro,
                Cidade = usuarioDto.Cidade,
                Estado = usuarioDto.Estado,
                Telefone = usuarioDto.Telefone,
                Cpf = usuarioDto.Cpf,
                Rg = usuarioDto.Rg
            };

            await _usuarioRepository.CriarAsync(usuario);

            return usuarioDto;
        }

        public string GerarSenha(int tamanho = 12)
        {
            const string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*";
            return string.Create(tamanho, caracteres, (res, chars) =>
            {
                for (int i = 0; i < res.Length; i++)
                {
                    res[i] = chars[RandomNumberGenerator.GetInt32(chars.Length)];
                }
            });
        }

        public async Task<string> GerarRegistro(int idPerfil)
        {
            string ano = DateTime.Now.ToString("yy");
            int mes = DateTime.Now.Month;
            int semestre = (mes < 6) ? 1 : 2;
            
            string prefixo = $"{idPerfil}{ano}{semestre}";

            string? ultimoRegistro = await _usuarioRepository.ObterUltimoRegistroPorPrefixoAsync(prefixo);

            int proximoSequencial = 1;

            if (!string.IsNullOrEmpty(ultimoRegistro))
            {
                string sequenciaStr = ultimoRegistro.Substring(prefixo.Length);
                
                if (int.TryParse(sequenciaStr, out int ultimoNumero))
                {
                    proximoSequencial = ultimoNumero + 1;
                }
            }

            return $"{prefixo}{proximoSequencial:D4}";
        }

        private string NormalizarParaEmail(string nomeCompleto)
        {
            if (string.IsNullOrWhiteSpace(nomeCompleto)) return string.Empty;

            var normalizedString = nomeCompleto.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            var nomeSemAcento = stringBuilder.ToString()
                .Normalize(NormalizationForm.FormC)
                .ToLower();

            var partes = nomeSemAcento.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (partes.Length == 0) return string.Empty;
            if (partes.Length == 1) return partes[0]; 

            var primeiroNome = partes[0];
            var ultimoNome = partes[partes.Length - 1];

            return $"{primeiroNome}{ultimoNome}";
        }

        public async Task<string> GerarEmailAsync(string nomeCompleto)
        {
            var nomeBase = NormalizarParaEmail(nomeCompleto);
            var dominio = "@edu.br";
            
            var emailTentativa = nomeBase + dominio;

            var emailsExistentes = await _usuarioRepository.BuscarPorEmailAsync(emailTentativa);

            if (emailsExistentes is null)
            {
                return emailTentativa;
            }

            int contador = 1;
            while (await _usuarioRepository.BuscarPorEmailAsync($"{nomeBase}{contador}{dominio}") is not null)
            {
                contador++;
            }

            return $"{nomeBase}{contador}{dominio}";
        }

        public async Task DeletarUsuarioAsync(int id)
        {
            await _usuarioRepository.DeletarAsync(id);
        }

        public async Task DesativarUsuarioAsync(int id)
        {
            var usuario = await _usuarioRepository.BuscarPorIdAsync(id)
                ?? throw new KeyNotFoundException("Usuário não encontrado.");
            usuario.Ativo = false;
            await _usuarioRepository.AtualizarAsync(usuario);
        }

        public async Task<UsuarioDto> ObterUsuarioPorIdAsync(int id)
        {
            var usuario = await _usuarioRepository.BuscarPorIdAsync(id);
            if (usuario == null)
            {
                return null;
            }

            return new UsuarioDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                IdPerfil = usuario.IdPerfil,
                Cep = usuario.Cep,
                Endereco = usuario.Endereco,
                Numero = usuario.Numero,
                Complemento = usuario.Complemento,
                Bairro = usuario.Bairro,
                Cidade = usuario.Cidade,
                Estado = usuario.Estado,
                Telefone = usuario.Telefone,
                Cpf = usuario.Cpf,
                Rg = usuario.Rg,
                Registro = usuario.Registro,
                Papel = ((PerfilEnum)usuario.IdPerfil).ToString(),
                Status = usuario.Ativo ? "Ativo" : "Inativo",
                FotoUrl = usuario.FotoUrl,
                NotificarTarefas = usuario.NotificarTarefas,
                NotificarAvisos = usuario.NotificarAvisos,
                NotificarNotas = usuario.NotificarNotas
            };
        }

        public async Task<string> AtualizarFotoAsync(int id, string fotoUrl)
        {
            var usuario = await _usuarioRepository.BuscarPorIdAsync(id) ?? throw new KeyNotFoundException("Usuário não encontrado.");
            usuario.FotoUrl = fotoUrl;
            await _usuarioRepository.AtualizarAsync(usuario);
            return fotoUrl;
        }

        public async Task<PreferenciasNotificacaoDto> AtualizarPreferenciasAsync(int id, PreferenciasNotificacaoDto dto)
        {
            var usuario = await _usuarioRepository.BuscarPorIdAsync(id) ?? throw new KeyNotFoundException("Usuário não encontrado.");
            usuario.NotificarTarefas = dto.NotificarTarefas;
            usuario.NotificarAvisos = dto.NotificarAvisos;
            usuario.NotificarNotas = dto.NotificarNotas;
            await _usuarioRepository.AtualizarAsync(usuario);
            return dto;
        }

        public async Task<UsuarioDto> AtualizarUsuarioAsync(int id, UsuarioDto usuarioDto)
        {
            var usuario = await _usuarioRepository.BuscarPorIdAsync(id);
            if (usuario == null)
            {
                return null;
            }

            usuario.Nome = usuarioDto.Nome;
            usuario.Cep = usuarioDto.Cep;
            usuario.Endereco = usuarioDto.Endereco;
            usuario.Numero = usuarioDto.Numero;
            usuario.Complemento = usuarioDto.Complemento;
            usuario.Bairro = usuarioDto.Bairro;
            usuario.Cidade = usuarioDto.Cidade;
            usuario.Estado = usuarioDto.Estado;
            usuario.Telefone = usuarioDto.Telefone;
            usuario.Cpf = usuarioDto.Cpf;
            usuario.Rg = usuarioDto.Rg;
            usuario.Ativo = usuarioDto.Status == "Ativo";

            await _usuarioRepository.AtualizarAsync(usuario);

            return usuarioDto;
        }

        public async Task<PagedResponse<UsuarioDto>> Execute(PaginacaoFiltroDto filtro)
        {
            var (usuarios, total) = await _usuarioRepository.ListarUsuariosPaginados(filtro);

            var listaDto = usuarios.Select(u => new UsuarioDto
            {
                Id = u.Id,
                Nome = u.Nome,
                Email = u.Email,
                IdPerfil = (int)u.IdPerfil,
                Registro = u.Registro,
                Papel = ((PerfilEnum)u.IdPerfil).ToString(),
                Status = u.Ativo ? "Ativo" : "Inativo",
                Telefone = u.Telefone,
                Cpf = u.Cpf,
                Rg = u.Rg,
                Cep = u.Cep,
                Endereco = u.Endereco,
                Numero = u.Numero,
                Complemento = u.Complemento,
                Bairro = u.Bairro,
                Cidade = u.Cidade,
                Estado = u.Estado,
                FotoUrl = u.FotoUrl,
                NotificarTarefas = u.NotificarTarefas,
                NotificarAvisos = u.NotificarAvisos,
                NotificarNotas = u.NotificarNotas
            }).ToList();

            return new PagedResponse<UsuarioDto>(listaDto, total, filtro.PaginaNumero, filtro.PaginaTamanho);
        }
    }
}
