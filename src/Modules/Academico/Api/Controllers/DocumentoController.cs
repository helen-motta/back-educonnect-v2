using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Modules.Academico.Application.UseCases;

namespace Modules.Academico.Api.Controllers
{
    [ApiController]
    [Route("api/documentos")]
    [Authorize]
    public class DocumentoController : ControllerBase
    {
        private readonly DocumentoUseCase _documentoUseCase;

        public DocumentoController(DocumentoUseCase documentoUseCase)
        {
            _documentoUseCase = documentoUseCase;
        }

        /// <summary>
        /// Gera um documento PDF para o usuário autenticado
        /// </summary>
        /// <param name="tipo">Tipo de documento: "matricula" ou "historico"</param>
        /// <returns>Arquivo PDF</returns>
        [HttpGet("gerar-pdf/{tipo}")]
        public async Task<IActionResult> GerarPdf(string tipo)
        {
            try
            {
                // Extrair o ID do usuário do token JWT
                var alunoIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                
                if (alunoIdClaim == null)
                {
                    return Unauthorized(new { message = "ID do usuário não encontrado no token" });
                }

                if (!int.TryParse(alunoIdClaim.Value, out int alunoId))
                {
                    return BadRequest(new { message = "ID do usuário inválido" });
                }

                byte[] pdfBytes;

                // Gerar o PDF conforme o tipo solicitado
                if (tipo.ToLower() == "matricula")
                {
                    pdfBytes = await _documentoUseCase.GerarAtestadoMatricula(alunoId);
                }
                else if (tipo.ToLower() == "historico")
                {
                    pdfBytes = await _documentoUseCase.GerarAtestadoMatricula(alunoId);
                    // TODO: Implementar GerarHistoricoAcademico quando necessário
                }
                else
                {
                    return BadRequest(new { message = "Tipo de documento inválido. Use 'matricula' ou 'historico'." });
                }

                return File(pdfBytes, "application/pdf", $"atestado_{tipo}_{alunoId}.pdf");
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Erro ao gerar documento", details = ex.Message });
            }
        }
    }
}