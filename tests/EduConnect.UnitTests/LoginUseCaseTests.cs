using System.Net;
using Microsoft.Extensions.Configuration;
using Modules.Autenticacao.Application.DTOs;
using Modules.Autenticacao.Application.UseCases;
using Modules.Autenticacao.Domain.Entities;
using Modules.Autenticacao.Domain.Interfaces;

namespace EduConnect.UnitTests;

public sealed class LoginUseCaseTests
{
    [Fact]
    public async Task CredenciaisVazias_RetornamBadRequestSemConsultarRepositorio()
    {
        var repository = new FakeUsuarioRepository();
        var useCase = CreateUseCase(repository);

        var (response, status) = await useCase.ExecutarAsync(new LoginRequest { Email = "", Senha = "" });

        Assert.Null(response);
        Assert.Equal(HttpStatusCode.BadRequest, status);
        Assert.Equal(0, repository.BuscasPorEmail);
    }

    [Fact]
    public async Task SenhaCorreta_RetornaTokenEResetaTentativas()
    {
        var usuario = CreateUser();
        usuario.TentativasFalhas = 2;
        var repository = new FakeUsuarioRepository(usuario);
        var useCase = CreateUseCase(repository);

        var (response, status) = await useCase.ExecutarAsync(new LoginRequest
        {
            Email = usuario.Email,
            Senha = "senha-correta"
        });

        Assert.Equal(HttpStatusCode.OK, status);
        Assert.NotNull(response);
        Assert.Equal("token-unitario", response.Token);
        Assert.Equal(usuario.Id, response.Usuario.Id);
        Assert.Equal(0, usuario.TentativasFalhas);
        Assert.NotNull(usuario.UltimoLogin);
        Assert.Equal(1, repository.Atualizacoes);
    }

    [Fact]
    public async Task TerceiraSenhaIncorreta_BloqueiaUsuario()
    {
        var usuario = CreateUser();
        usuario.TentativasFalhas = 2;
        var repository = new FakeUsuarioRepository(usuario);
        var useCase = CreateUseCase(repository);

        var (response, status) = await useCase.ExecutarAsync(new LoginRequest
        {
            Email = usuario.Email,
            Senha = "senha-incorreta"
        });

        Assert.Null(response);
        Assert.Equal(HttpStatusCode.Unauthorized, status);
        Assert.Equal(3, usuario.TentativasFalhas);
        Assert.True(usuario.EstaBloqueado());
        Assert.Equal(1, repository.Atualizacoes);
    }

    [Fact]
    public async Task UsuarioInativo_RetornaForbiddenSemAlterarUsuario()
    {
        var usuario = CreateUser();
        usuario.Ativo = false;
        var repository = new FakeUsuarioRepository(usuario);
        var useCase = CreateUseCase(repository);

        var (response, status) = await useCase.ExecutarAsync(new LoginRequest
        {
            Email = usuario.Email,
            Senha = "senha-correta"
        });

        Assert.Null(response);
        Assert.Equal(HttpStatusCode.Forbidden, status);
        Assert.Equal(0, repository.Atualizacoes);
    }

    [Fact]
    public async Task ResetExpirado_LimpaTokenERejeitaAlteracao()
    {
        var usuario = CreateUser();
        usuario.ResetSenhaToken = "token-expirado";
        usuario.DataExpiraTokenResetSenha = DateTime.UtcNow.AddMinutes(-1);
        var repository = new FakeUsuarioRepository(usuario);
        var useCase = CreateUseCase(repository);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.EfetuarResetAsync(
            new ResetSenhaDto { Email = usuario.Email, Token = "token-expirado", NovaSenha = "nova-senha" }));

        Assert.Equal("Token expirado", exception.Message);
        Assert.Null(usuario.ResetSenhaToken);
        Assert.Null(usuario.DataExpiraTokenResetSenha);
        Assert.Equal(1, repository.Atualizacoes);
    }

    [Fact]
    public async Task ResetValido_AlteraSenhaEInvalidaToken()
    {
        var usuario = CreateUser();
        usuario.ResetSenhaToken = "token-valido";
        usuario.DataExpiraTokenResetSenha = DateTime.UtcNow.AddMinutes(10);
        var repository = new FakeUsuarioRepository(usuario);
        var useCase = CreateUseCase(repository);

        await useCase.EfetuarResetAsync(new ResetSenhaDto
        {
            Email = usuario.Email,
            Token = "token-valido",
            NovaSenha = "nova-senha-segura"
        });

        Assert.True(BCrypt.Net.BCrypt.Verify("nova-senha-segura", usuario.SenhaHash));
        Assert.Null(usuario.ResetSenhaToken);
        Assert.Null(usuario.DataExpiraTokenResetSenha);
        Assert.Equal(1, repository.Atualizacoes);
    }

    [Fact]
    public async Task SolicitarReset_PersisteTokenEEnviaLink()
    {
        var usuario = CreateUser();
        var repository = new FakeUsuarioRepository(usuario);
        var emailService = new FakeEmailService();
        var useCase = CreateUseCase(repository, emailService);

        var token = await useCase.SolicitarResetAsync(new EsqueceuSenhaDto { Email = usuario.Email });

        Assert.False(string.IsNullOrWhiteSpace(token));
        Assert.Equal(token, usuario.ResetSenhaToken);
        Assert.InRange(usuario.DataExpiraTokenResetSenha!.Value,
            DateTime.UtcNow.AddMinutes(14), DateTime.UtcNow.AddMinutes(16));
        Assert.Equal(usuario.Email, emailService.Destinatario);
        Assert.Contains(token!, emailService.Link);
        Assert.Contains("http://localhost:5173/redefinir-senha", emailService.Link);
    }

    private static LoginUseCase CreateUseCase(
        FakeUsuarioRepository repository,
        FakeEmailService? emailService = null)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["UrlSettings:FrontendUrl"] = "http://localhost:5173"
            })
            .Build();

        return new LoginUseCase(repository, new FakeTokenService(), emailService ?? new FakeEmailService(), configuration);
    }

    private static Usuario CreateUser() => new()
    {
        Id = 10,
        Nome = "Usuário de Teste",
        Email = "teste@educonnect.local",
        IdPerfil = 3,
        Ativo = true,
        SenhaHash = BCrypt.Net.BCrypt.HashPassword("senha-correta")
    };

    private sealed class FakeTokenService : ITokenService
    {
        public string GerarToken(Usuario usuario) => "token-unitario";
        public bool ValidarToken(string token) => token == "token-unitario";
    }

    private sealed class FakeEmailService : IEmailService
    {
        public string Destinatario { get; private set; } = string.Empty;
        public string Link { get; private set; } = string.Empty;

        public Task EnviarEmailResetSenhaAsync(string email, string nome, string resetToken, string resetLink)
        {
            Destinatario = email;
            Link = resetLink;
            return Task.CompletedTask;
        }

        public Task EnviarEmailAsync(string destinatario, string assunto, string conteudo) => Task.CompletedTask;
    }
}
