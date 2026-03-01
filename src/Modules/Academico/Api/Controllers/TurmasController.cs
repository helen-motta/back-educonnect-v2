using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modules.Academico.Application.UseCases;
using Shared.Infrastructure;

[ApiController]
[Route("api/turmas")]
public class TurmasController : ControllerBase
{
    private readonly AppDbContext _context;

    private readonly TurmasUseCase _turmasUseCase;

    public TurmasController(TurmasUseCase turmasUseCase)
    {
        _turmasUseCase = turmasUseCase;
    }

    [HttpGet]
    public async Task<IActionResult> GetTurmas([FromQuery] TurmaListagemDto filtro)
    {
        var response = await _turmasUseCase.Execute(filtro);
        return Ok(response);
    }
}