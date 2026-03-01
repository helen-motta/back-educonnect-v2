using SendGrid;
using SendGrid.Helpers.Mail;
using Modules.Autenticacao.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Modules.Autenticacao.Infrastructure.Services
{
    public class SendGridEmailService : IEmailService
    {
        private readonly SendGridClient _sendGridClient;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly IConfiguration _configuration;

        public SendGridEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            var apiKey = configuration["SendGrid:ApiKey"];
            _fromEmail = configuration["SendGrid:FromEmail"];
            _fromName = configuration["SendGrid:FromName"] ?? "EduConnect";

            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException("SendGrid API key is not configured");

            if (string.IsNullOrWhiteSpace(_fromEmail))
                throw new ArgumentException("SendGrid FromEmail is not configured");

            _sendGridClient = new SendGridClient(apiKey);
        }

        public async Task EnviarEmailResetSenhaAsync(string email, string nome, string resetToken, string resetLink)
        {
            var assunto = "Redefinição de senha @ EduConnect";
            var conteudo = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333333; background-color: #f9f9fb; margin: 0; padding: 0; }}
        .wrapper {{ background-color: #f9f9fb; padding: 30px 0; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.05); border: 1px solid #eeeeee; }}
        .header {{ background-color: #d8f17b; padding: 30px; text-align: center; border-bottom: 3px solid #d8f17b; }}
        .header h1 {{ color: #222222; margin: 0; font-size: 24px; text-transform: uppercase; letter-spacing: 2px; }}
        .content {{ padding: 40px; background-color: #ffffff; color: #444444; }}
        .content p {{ margin-bottom: 20px; font-size: 16px; color: #555555; }}
        .content strong {{ color: #222222; }}
        .button-container {{ text-align: center; margin: 30px 0; }}
        .button {{ 
            display: inline-block; 
            background-color: #d8f17b; 
            color: #222222 !important; 
            padding: 14px 30px; 
            text-decoration: none; 
            border-radius: 8px; 
            font-weight: bold; 
            font-size: 16px;
            box-shadow: 0 2px 5px rgba(216, 241, 123, 0.4);
        }}
        .link-box {{ 
            background-color: #f4f4f4; 
            padding: 15px; 
            border-radius: 6px; 
            word-break: break-all; 
            font-size: 12px; 
            color: #777;
            border: 1px solid #dddddd;
            margin-top: 20px;
        }}
        .warning {{ 
            border-top: 1px solid #eeeeee; 
            margin-top: 30px; 
            padding-top: 20px; 
            color: #888888; 
            font-size: 13px; 
            font-style: italic; 
        }}
        .footer {{ background-color: #fcfcfc; padding: 20px; text-align: center; font-size: 12px; color: #999999; border-top: 1px solid #eeeeee; }}
    </style>
</head>
<body>
    <div class='wrapper'>
        <div class='container'>
            <div class='header'>
                <h1>EduConnect</h1>
            </div>
            <div class='content'>
                <p>Olá, <strong>{nome}</strong>!</p>
                <p>Recebemos uma solicitação para redefinir a sua senha de acesso à plataforma.</p>
                <p>Para prosseguir com a alteração, clique no botão de destaque abaixo:</p>
                
                <div class='button-container'>
                    <a href='{resetLink}' class='button'>REDEFINIR MINHA SENHA</a>
                </div>

                <p>Este link é válido por apenas <strong>30 minutos</strong>.</p>
                
                <p>Se o botão não funcionar, copie o link abaixo:</p>
                <div class='link-box'>{resetLink}</div>

                <div class='warning'>
                    <p>Se você não solicitou esta alteração, pode ignorar este e-mail com segurança. Sua senha atual não será alterada.</p>
                </div>
            </div>
            <div class='footer'>
                <p>&copy; 2026 EduConnect - Gestão Educacional Inteligente</p>
                <p>Este é um e-mail automático, por favor não responda.</p>
            </div>
        </div>
    </div>
</body>
</html>";

            await EnviarEmailAsync(email, assunto, conteudo);
        }

        public async Task EnviarEmailAsync(string destinatario, string assunto, string conteudo)
        {
            try
            {
                var from = new EmailAddress(_fromEmail, _fromName);
                var to = new EmailAddress("helenmottab@gmail.com");
                var msg = MailHelper.CreateSingleEmail(from, to, assunto, conteudo, conteudo);

                var response = await _sendGridClient.SendEmailAsync(msg);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Body.ReadAsStringAsync();
                    throw new InvalidOperationException($"Falha ao enviar e-mail: {response.StatusCode}. Detalhes: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao enviar e-mail para {destinatario}: {ex.Message}", ex);
            }
        }
    }
}
