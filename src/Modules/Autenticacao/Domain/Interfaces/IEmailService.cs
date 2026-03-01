namespace Modules.Autenticacao.Domain.Interfaces
{
    public interface IEmailService
    {
        Task EnviarEmailResetSenhaAsync(string email, string nome, string resetToken, string resetLink);
        Task EnviarEmailAsync(string destinatario, string assunto, string conteudo);
    }
}
