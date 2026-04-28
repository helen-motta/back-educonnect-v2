using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modules.Academico.Application.DTOs;
using Modules.Academico.Application.UseCases;
using Shared.Api;
using Shared.Infrastructure;

[ApiController]
[Route("api/[controller]")]
public class TurmasController : BaseApiController
{
    private readonly TurmasUseCase _turmasUseCase;
    private readonly AuditoriaUseCase _auditoriaUseCase;

    public TurmasController(TurmasUseCase turmasUseCase, AuditoriaUseCase auditoriaUseCase)
    {
        _turmasUseCase = turmasUseCase;
        _auditoriaUseCase = auditoriaUseCase;
    }

    [HttpGet]
    public async Task<IActionResult> GetTurmas([FromQuery] TurmaListagemDto filtro)
    {
        var response = await _turmasUseCase.Execute(filtro);
        return Ok(response);
    }

    [HttpGet("{turmaId}")]
    public async Task<IActionResult> GetTurmaById(int turmaId)
    {
        var response = await _turmasUseCase.GetTurmasById(turmaId);

        return Ok(response);
    }

    [HttpGet("aluno/{alunoId}/horarios")]
    public async Task<IActionResult> GetHorariosPorAluno(int alunoId)
    {
        var response = await _turmasUseCase.GetHorariosPorAluno(alunoId);

        return Ok(response);
    }

    [HttpGet("professor")]
    public async Task<IActionResult> GetTurmasPorProfessor()
    {
        var professorId = int.Parse(User.FindFirst("sub")?.Value ?? "0");

        var response = await _turmasUseCase.GetTurmasPorProfessor(professorId);

        return Ok(response);
    }

}