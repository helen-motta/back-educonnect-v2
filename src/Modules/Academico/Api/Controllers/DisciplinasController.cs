using Microsoft.AspNetCore.Mvc;
using Modules.Academico.Application.DTOs;
using Modules.Academico.Application.UseCases;
using Modules.Academico.Domain.Entities;
using Modules.Autenticacao.Application.UseCases;
using Shared.Api;
using src.Modules.Academico.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace src.Modules.Academico.Api.Controllers.DisciplinasController
{
    [ApiController]
    [Route("api/[controller]")]
    public class DisciplinasController : BaseApiController
    {
        private readonly DisciplinasUseCase _disciplinasUseCase;
        private readonly AuditoriaUseCase _auditoriaUseCase;

        public DisciplinasController(DisciplinasUseCase disciplinasUseCase, AuditoriaUseCase auditoriaUseCase)
        {
            _disciplinasUseCase = disciplinasUseCase;
            _auditoriaUseCase = auditoriaUseCase;
        }

        [HttpGet("/{idCurso}")]
        public async Task<IActionResult> ObterPorIdCurso(int idCurso)
        {
            var disciplinas = await _disciplinasUseCase.ObterPorIdCursoAsync(idCurso);

            return Ok(disciplinas);
        }

        [HttpPost]
        public async Task<ActionResult<Disciplina>> CriarDisciplina([FromBody] CriarDisciplinaRequest request)
        {
            if (request == null)
                return BadRequest("Dados da disciplina são obrigatórios.");

            if (string.IsNullOrWhiteSpace(request.Nome))
                return BadRequest("Nome da disciplina é obrigatório.");

            if (string.IsNullOrWhiteSpace(request.Codigo))
                return BadRequest("Código da disciplina é obrigatório.");

            if (request.CargaHoraria <= 0)
                return BadRequest("Carga horária deve ser maior que zero.");

            if (request.Creditos.HasValue && request.Creditos.Value <= 0)
                return BadRequest("Créditos devem ser maior que zero.");

            if (request.IdCurso <= 0)
                return BadRequest("ID do curso é obrigatório.");

            var disciplina = await _disciplinasUseCase.CriarDisciplinaAsync(request);

            await _auditoriaUseCase.RegistrarAsync(new RegistrarAuditoriaRequestDto
            {
                TabelaNome = "disciplinas",
                EntidadeId = disciplina.Id.ToString(),
                Operacao = "INSERT",
                DadosAtual = disciplina,
                UsuarioId = ObterUsuarioId(),
                EnderecoIp = ObterEnderecoIp(),
                UserAgent = ObterUserAgent()
            });

            return Created($"/api/disciplinas/{disciplina.Id}", disciplina);
        }

    }
}