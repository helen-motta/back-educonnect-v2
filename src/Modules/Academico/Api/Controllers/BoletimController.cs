using Microsoft.AspNetCore.Mvc;
using Modules.Academico.Application.UseCases;

namespace Modules.Academico.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BoletimController : ControllerBase
    {
        private readonly TurmasUseCase _turmasUseCase;

        public BoletimController(TurmasUseCase turmasUseCase)
        {
            _turmasUseCase = turmasUseCase;
        }

        /// <summary>
        /// Retorna as notas e a frequência do aluno em todas as turmas em que está inscrito.
        /// </summary>
        [HttpGet("aluno/{alunoId:int}")]
        public async Task<IActionResult> GetBoletimPorAluno(int alunoId)
        {
            var result = await _turmasUseCase.GetNotasEFrequenciaPorAluno(alunoId);
            return Ok(result);
        }
    }
}

