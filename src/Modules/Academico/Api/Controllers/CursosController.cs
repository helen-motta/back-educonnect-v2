using Microsoft.AspNetCore.Mvc;
using Modules.Academico.Application.DTOs;
using Modules.Academico.Application.UseCases;
using Modules.Autenticacao.Application.UseCases;
using Shared.Api;
using src.Modules.Academico.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace src.Modules.Academico.Api.Controllers.CursosController
{
    [ApiController]
    [Route("api/[controller]")]
    public class CursosController : BaseApiController
    {
        private readonly CursosUseCase _cursosUseCase;

        private readonly AuditoriaUseCase _auditoriaUseCase;
        public CursosController(CursosUseCase cursosUseCase, AuditoriaUseCase auditoriaUseCase)
        {
            _cursosUseCase = cursosUseCase;
            _auditoriaUseCase = auditoriaUseCase;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Curso>> ObterPorId(int id)
        {
            if (id <= 0)
                return BadRequest("ID deve ser maior que zero.");

            var curso = await _cursosUseCase.ObterPorIdAsync(id);
            
            if (curso == null)
                return NotFound($"Curso com ID {id} não encontrado.");

            return Ok(curso);
        }

        [HttpGet]
        public async Task<IActionResult> GetCursos([FromQuery] PaginacaoCursosDto filtro)
        {
            var response = await _cursosUseCase.Execute(filtro);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<Curso>> CriarCurso([FromBody] CriarCursoRequest request)
        {
            if (request == null)
                return BadRequest("Dados do curso são obrigatórios.");

            if (string.IsNullOrWhiteSpace(request.Nome))
                return BadRequest("Nome do curso é obrigatório.");

            if (string.IsNullOrWhiteSpace(request.Codigo))
                return BadRequest("Código do curso é obrigatório.");

            if (request.CargaHoraria <= 0)
                return BadRequest("Carga horária deve ser maior que zero.");

            if (request.IdCoordenador <= 0)
                return BadRequest("ID do coordenador é obrigatório.");

            var curso = await _cursosUseCase.CriarCursoAsync(request);
            return CreatedAtAction(nameof(ObterPorId), new { id = curso.Id }, curso);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Curso>> AtualizarCurso(int id, [FromBody] CriarCursoRequest request)
        {
            if (id <= 0)
                return BadRequest("ID deve ser maior que zero.");

            if (request == null)
                return BadRequest("Dados do curso são obrigatórios.");

            if (string.IsNullOrWhiteSpace(request.Nome))
                return BadRequest("Nome do curso é obrigatório.");

            if (string.IsNullOrWhiteSpace(request.Codigo))
                return BadRequest("Código do curso é obrigatório.");

            if (request.CargaHoraria <= 0)
                return BadRequest("Carga horária deve ser maior que zero.");

            if (request.IdCoordenador <= 0)
                return BadRequest("ID do coordenador é obrigatório.");

            var cursoAtualizado = await _cursosUseCase.AtualizarCursoAsync(id, request);

            await _auditoriaUseCase.RegistrarAsync(new RegistrarAuditoriaRequestDto
            {
                TabelaNome = "cursos",
                EntidadeId = cursoAtualizado.Id.ToString(),
                Operacao = "UPDATE",
                DadosAtual = cursoAtualizado,
                UsuarioId = ObterUsuarioId(),
                EnderecoIp = ObterEnderecoIp(),
                UserAgent = ObterUserAgent()
            });

            if (cursoAtualizado == null)
                return NotFound($"Curso com ID {id} não encontrado.");

            return Ok(cursoAtualizado);
        }
    }
}