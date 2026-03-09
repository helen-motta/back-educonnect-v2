using Microsoft.AspNetCore.Mvc;
using Modules.Autenticacao.Domain.Interfaces;

namespace Modules.Academico.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AvisosController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public AvisosController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        /// <summary>
        /// Envia um aviso do professor por e-mail (dados mockados por enquanto).
        /// </summary>
        [HttpPost("enviar-aviso")]
        public async Task<IActionResult> EnviarAvisoProfessor()
        {
            // Dados mockados por enquanto
            var emailAluno = "helenmottab@gmail.com";
            var nomeAluno = "Helen";
            var nomeProfessor = "Prof. João Silva";
            var disciplina = "Programação Web";
            var tituloAviso = "Aviso de Prova";
            var mensagemAviso = "Lembrete: a prova de Programação Web será na próxima terça-feira, às 19h, sala B101.";

            var assunto = $"[Aviso] {disciplina} - {tituloAviso}";

            var conteudoHtml = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f9f9fb; margin: 0; padding: 0; }}
        .wrapper {{ padding: 30px 0; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 12px; box-shadow: 0 4px 15px rgba(0,0,0,0.05); border: 1px solid #eeeeee; overflow: hidden; }}
        .header {{ background-color: #d8f17b; padding: 24px 30px; }}
        .header h1 {{ margin: 0; font-size: 22px; color: #222222; }}
        .content {{ padding: 30px; color: #444444; }}
        .content p {{ margin-bottom: 16px; font-size: 15px; }}
        .meta {{ font-size: 13px; color: #777777; margin-bottom: 16px; }}
        .footer {{ background-color: #fcfcfc; padding: 16px 24px; text-align: center; font-size: 12px; color: #999999; border-top: 1px solid #eeeeee; }}
    </style>
</head>
<body>
    <div class='wrapper'>
        <div class='container'>
            <div class='header'>
                <h1>Aviso do professor</h1>
            </div>
            <div class='content'>
                <p>Olá, <strong>{nomeAluno}</strong>!</p>
                <p class='meta'>
                    <strong>Professor:</strong> {nomeProfessor}<br/>
                    <strong>Disciplina:</strong> {disciplina}
                </p>
                <p><strong>{tituloAviso}</strong></p>
                <p>{mensagemAviso}</p>
            </div>
            <div class='footer'>
                <p>&copy; 2026 EduConnect - Avisos Acadêmicos</p>
            </div>
        </div>
    </div>
</body>
</html>";

            bool emailEnviado = false;
            string? erroEnvio = null;

            try
            {
                await _emailService.EnviarEmailAsync(emailAluno, assunto, conteudoHtml);
                emailEnviado = true;
            }
            catch (Exception ex)
            {
                // Como ainda estamos mockando, não vamos quebrar o endpoint;
                // apenas retornamos a informação de erro no payload.
                erroEnvio = ex.Message;
            }

            return Ok(new
            {
                message = emailEnviado
                    ? "Aviso enviado (dados mockados)."
                    : "Aviso processado, mas houve erro ao enviar o e-mail (mock).",
                destinatario = emailAluno,
                professor = nomeProfessor,
                disciplina,
                titulo = tituloAviso,
                emailEnviado,
                erroEnvio
            });
        }
    }
}

