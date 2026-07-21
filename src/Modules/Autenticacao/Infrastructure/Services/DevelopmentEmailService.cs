using Modules.Autenticacao.Domain.Interfaces;

namespace Modules.Autenticacao.Infrastructure.Services;

public sealed class DevelopmentEmailService : IEmailService
{
    private readonly ILogger<DevelopmentEmailService> _logger;

    public DevelopmentEmailService(ILogger<DevelopmentEmailService> logger) => _logger = logger;

    public Task EnviarEmailResetSenhaAsync(string email, string nome, string resetToken, string resetLink)
    {
        _logger.LogInformation("E-mail de redefinição para {Email}. Link local: {ResetLink}; token: {Token}", email, resetLink, resetToken);
        return Task.CompletedTask;
    }

    public Task EnviarEmailAsync(string destinatario, string assunto, string conteudo)
    {
        _logger.LogInformation("E-mail local para {Destinatario}: {Assunto}\n{Conteudo}", destinatario, assunto, conteudo);
        return Task.CompletedTask;
    }
}
