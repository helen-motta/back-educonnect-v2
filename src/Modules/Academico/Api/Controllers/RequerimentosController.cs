using Microsoft.AspNetCore.Mvc;
using Modules.Academico.Application.DTOs;
using Modules.Academico.Application.UseCases;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace src.Modules.Academico.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequerimentosController : ControllerBase
    {
        private readonly RequerimentosUseCase _requerimentosUseCase;

        public RequerimentosController(RequerimentosUseCase requerimentosUseCase)
        {
            _requerimentosUseCase = requerimentosUseCase;
        }

        [HttpPost]
        public async Task<ActionResult<Requerimentos>> CriarRequerimento([FromBody] NovoRequerimentoDto request, [FromQuery] int idUsuario)
        {
            if (request == null)
                return BadRequest("Dados do requerimento são obrigatórios.");

            if (string.IsNullOrWhiteSpace(request.Tipo))
                return BadRequest("Tipo de solicitação é obrigatório.");

            if (idUsuario <= 0)
                return BadRequest("ID do usuário é obrigatório.");

            var requerimento = await _requerimentosUseCase.CriarRequerimentoAsync(idUsuario, request.Tipo, request.Observacao);
            return CreatedAtAction(nameof(ObterPorId), new { id = requerimento.Id }, requerimento);
        }

        [HttpGet("usuario/{idUsuario}")]
        public async Task<ActionResult<List<Requerimentos>>> ObterPorUsuario(int idUsuario)
        {
            if (idUsuario <= 0)
                return BadRequest("ID do usuário deve ser maior que zero.");

            var requerimentos = await _requerimentosUseCase.ObterRequerimentosPorUsuarioAsync(idUsuario);
            return Ok(requerimentos);
        }

        [HttpGet]
        public async Task<IActionResult> ListarRequerimentos([FromQuery] PaginacaoRequerimentosDto filtro)
        {
            var response = await _requerimentosUseCase.ListarRequerimentosPaginadosAsync(filtro);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Requerimentos>> ObterPorId(int id)
        {
            if (id <= 0)
                return BadRequest("ID deve ser maior que zero.");

            var requerimento = await _requerimentosUseCase.ObterRequerimentoPorIdAsync(id);
            
            if (requerimento == null)
                return NotFound($"Requerimento com ID {id} não encontrado.");

            return Ok(requerimento);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Requerimentos>> AtualizarStatus(int id, [FromBody] AtualizarRequerimentoStatusDto request)
        {
            if (id <= 0)
                return BadRequest("ID deve ser maior que zero.");

            if (request == null)
                return BadRequest("Dados para atualização são obrigatórios.");

            if (string.IsNullOrWhiteSpace(request.Status))
                return BadRequest("Status é obrigatório.");

            var requerimento = await _requerimentosUseCase.ObterRequerimentoPorIdAsync(id);
            if (requerimento == null)
                return NotFound($"Requerimento com ID {id} não encontrado.");

            await _requerimentosUseCase.AtualizarStatusRequerimentoAsync(id, request.Status, request.RespostaAdmin);

            var requerimentoAtualizado = await _requerimentosUseCase.ObterRequerimentoPorIdAsync(id);
            return Ok(requerimentoAtualizado);
        }
    }
}
