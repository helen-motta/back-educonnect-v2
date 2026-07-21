using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Academico.Application.DTOs;
using Modules.Academico.Application.UseCases;

namespace Modules.Academico.Api.Controllers;

[ApiController, Authorize, Route("api/atividades")]
public sealed class AtividadesController : ControllerBase
{
    private readonly PortalUseCase _useCase;
    public AtividadesController(PortalUseCase useCase) => _useCase = useCase;
    [HttpGet] public async Task<IActionResult> Get() => Ok(await _useCase.ObterAtividadesAsync(UserId()));
    [HttpPost] public async Task<IActionResult> Post(SalvarAtividadeDto request) => Ok(await _useCase.CriarAtividadeAsync(UserId(), request));
    [HttpPut("{id:int}")] public async Task<IActionResult> Put(int id, SalvarAtividadeDto request) => Ok(await _useCase.AtualizarAtividadeAsync(UserId(), id, request));
    [HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id) { await _useCase.RemoverAtividadeAsync(UserId(), id); return NoContent(); }
    [HttpPut("{id:int}/entregas/{alunoId:int}")]
    public async Task<IActionResult> Grade(int id, int alunoId, AvaliarEntregaDto request) { await _useCase.AvaliarEntregaAsync(UserId(), id, alunoId, request); return NoContent(); }
    private int UserId() => int.TryParse(User.FindFirst("sub")?.Value, out var id) ? id : throw new UnauthorizedAccessException();
}
