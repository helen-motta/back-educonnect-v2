using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Academico.Application.DTOs;
using Modules.Academico.Application.UseCases;

namespace Modules.Academico.Api.Controllers;

[ApiController, Authorize, Route("api/turmas")]
public sealed class TurmasController : ControllerBase
{
    private readonly TurmasUseCase _useCase;
    public TurmasController(TurmasUseCase useCase) => _useCase = useCase;
    [HttpGet] public async Task<IActionResult> Get([FromQuery] TurmaListagemDto filtro) => Ok(await _useCase.Execute(filtro));
    [HttpGet("{id:int}")] public async Task<IActionResult> GetById(int id) => (await _useCase.GetTurmasById(id)) is { } item ? Ok(item) : NotFound();
    [HttpGet("aluno/{alunoId:int}/horarios")] public async Task<IActionResult> Schedule(int alunoId) => Ok(await _useCase.GetHorariosPorAluno(alunoId));
    [HttpGet("professor")] public async Task<IActionResult> Professor() => Ok(await _useCase.GetTurmasPorProfessor(UserId()));
    [HttpPost] public async Task<IActionResult> Post(SalvarTurmaDto request) => Ok(await _useCase.CriarAsync(request));
    [HttpPut("{id:int}")] public async Task<IActionResult> Put(int id, SalvarTurmaDto request) { await _useCase.AtualizarAsync(id, request); return NoContent(); }
    [HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id) { await _useCase.RemoverAsync(id); return NoContent(); }
    private int UserId() => int.TryParse(User.FindFirst("sub")?.Value, out var id) ? id : throw new UnauthorizedAccessException();
}
