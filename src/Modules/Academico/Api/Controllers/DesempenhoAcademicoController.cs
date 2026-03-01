using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DesempenhoAcademicoMock.Api.Controllers
{
    [ApiController]
    [Route("api/desempenho-academico")]
    public class DesempenhoAcademicoController : ControllerBase
    {
        private readonly DesempenhoAcademicoService _service;

        public DesempenhoAcademicoController()
        {
            _service = new DesempenhoAcademicoService();
        }

        [HttpGet("calcular")]
        public IActionResult CalcularDesempenho(
            [FromQuery] int alunoId,
            [FromQuery] int disciplinaId,
            [FromQuery] string periodoLetivo)
        {
            try
            {
                // ==========================
                // MOCKS (simulando banco)
                // ==========================

                var disciplina = ObterDisciplinaMock(disciplinaId);
                if (disciplina is null)
                    return NotFound(new { mensagem = "Disciplina não encontrada" });

                var avaliacoes = ObterAvaliacoesMock(alunoId, disciplinaId, periodoLetivo);
                if (!avaliacoes.Any())
                    return NotFound(new { mensagem = "Nenhuma avaliação encontrada" });

                var frequencia = ObterFrequenciaMock(alunoId, disciplinaId, periodoLetivo);
                if (frequencia is null)
                    return NotFound(new { mensagem = "Frequência não encontrada" });

                // ==========================
                // PROCESSAMENTO
                // ==========================

                var resultado = _service.Calcular(disciplina, avaliacoes, frequencia);

                // ==========================
                // RESPONSE
                // ==========================
                return Ok(new
                {
                    alunoId,
                    disciplinaId,
                    periodoLetivo,
                    mediaFinal = resultado.MediaFinal.Valor,
                    frequencia = resultado.Frequencia.Valor,
                    situacao = resultado.Situacao.ToString()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensagem = "Erro ao calcular desempenho",
                    detalhe = ex.Message
                });
            }
        }

        // ==========================================================
        // MOCKS
        // ==========================================================

        private Disciplina? ObterDisciplinaMock(int disciplinaId)
        {
            var disciplinas = new List<Disciplina>
            {
                new Disciplina(1, "Matemática", Nota.Criar(6), Percentual.Criar(75)),
                new Disciplina(2, "Algoritmos", Nota.Criar(6), Percentual.Criar(75))
            };

            return disciplinas.FirstOrDefault(d => d.Id == disciplinaId);
        }

        private List<Avaliacao> ObterAvaliacoesMock(
            int alunoId,
            int disciplinaId,
            string periodoLetivo)
        {
            // Cenários diferentes para testes
            return disciplinaId switch
            {
                // APROVADO
                1 => new List<Avaliacao>
                {
                    new Avaliacao(1, 1, "Prova 1", 8.0m, TipoAvaliacao.Prova, true),
                    new Avaliacao(2, 1, "Prova 2", 7.0m, TipoAvaliacao.Prova, true),
                    new Avaliacao(3, 1, "Trabalho", 9.0m, TipoAvaliacao.Trabalho, true)
                },

                // REPROVADO POR NOTA
                2 => new List<Avaliacao>
                {
                    new Avaliacao(4, 2, "Prova 1", 4.0m, TipoAvaliacao.Prova, true),
                    new Avaliacao(5, 2, "Prova 2", 5.0m, TipoAvaliacao.Prova, true),
                    new Avaliacao(6, 2, "Trabalho", 6.0m, TipoAvaliacao.Trabalho, true)
                },

                _ => new List<Avaliacao>()
            };
        }

        private Frequencia? ObterFrequenciaMock(
            int alunoId,
            int disciplinaId,
            string periodoLetivo)
        {
            return disciplinaId switch
            {
                // Frequência OK
                1 => new Frequencia(1, 48, 60, true),

                // Frequência OK (reprova só por nota)
                2 => new Frequencia(2, 50, 60, true),

                _ => null
            };
        }
    }
}
