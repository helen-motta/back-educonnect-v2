using Microsoft.AspNetCore.Mvc;
using Modules.Academico.Application.DTOs;
using Modules.Academico.Application.UseCases;
using Shared.Api;

namespace Modules.Academico.Api.Controllers
{
    [ApiController]
    [Route("api/professor")]
    public class ProfessorController : BaseApiController
    {
        private readonly ProfessorLancamentosUseCase _useCase;

        public ProfessorController(ProfessorLancamentosUseCase useCase)
        {
            _useCase = useCase;
        }

        [HttpPost("avaliacoes")]
        public async Task<IActionResult> CriarAvaliacao([FromBody] CriarAvaliacaoProfessorRequestDto request)
        {
            if (!TryObterProfessorId(out var professorId))
                return Unauthorized(new { message = "Token inválido ou sem claim sub." });

            try
            {
                var avaliacao = await _useCase.CriarAvaliacaoAsync(professorId, request);
                return Created($"/api/professor/avaliacoes/{avaliacao.Id}", avaliacao);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpGet("turmas/{turmaId}/avaliacoes")]
        public async Task<IActionResult> ListarAvaliacoesPorTurma(int turmaId)
        {
            if (!TryObterProfessorId(out var professorId))
                return Unauthorized(new { message = "Token inválido ou sem claim sub." });

            try
            {
                var avaliacoes = await _useCase.ListarAvaliacoesPorTurmaAsync(professorId, turmaId);
                return Ok(avaliacoes);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPost("faltas")]
        public async Task<IActionResult> LancarFalta([FromBody] LancarFrequenciaProfessorRequestDto request)
        {
            if (!TryObterProfessorId(out var professorId))
                return Unauthorized(new { message = "Token inválido ou sem claim sub." });

            try
            {
                var frequencia = await _useCase.LancarFrequenciaAsync(professorId, request);
                return Created($"/api/professor/faltas/{frequencia.Id}", frequencia);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpGet("matriculas/{matriculaId}/faltas")]
        public async Task<IActionResult> ListarFaltasPorMatricula(int matriculaId)
        {
            if (!TryObterProfessorId(out var professorId))
                return Unauthorized(new { message = "Token inválido ou sem claim sub." });

            try
            {
                var frequencias = await _useCase.ListarFrequenciasPorMatriculaAsync(professorId, matriculaId);
                return Ok(frequencias);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPost("notas")]
        public async Task<IActionResult> LancarNota([FromBody] LancarNotaProfessorRequestDto request)
        {
            if (!TryObterProfessorId(out var professorId))
                return Unauthorized(new { message = "Token inválido ou sem claim sub." });

            try
            {
                var nota = await _useCase.LancarNotaAsync(professorId, request);
                return Created($"/api/professor/notas/{nota.Id}", nota);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpGet("matriculas/{matriculaId}/notas")]
        public async Task<IActionResult> ListarNotasPorMatricula(int matriculaId)
        {
            if (!TryObterProfessorId(out var professorId))
                return Unauthorized(new { message = "Token inválido ou sem claim sub." });

            try
            {
                var notas = await _useCase.ListarNotasPorMatriculaAsync(professorId, matriculaId);
                return Ok(notas);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        private bool TryObterProfessorId(out int professorId)
        {
            return int.TryParse(User.FindFirst("sub")?.Value, out professorId) && professorId > 0;
        }
    }
}
