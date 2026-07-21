using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Academico.Application.DTOs;
using Modules.Academico.Application.UseCases;

namespace Modules.Academico.Api.Controllers;

[ApiController, Route("api/matriculas")]
public sealed class MatriculasController : ControllerBase
{
    private readonly PortalUseCase _useCase;
    public MatriculasController(PortalUseCase useCase) => _useCase = useCase;
    [AllowAnonymous, HttpGet("cursos-disponiveis")] public async Task<IActionResult> Courses() => Ok(await _useCase.ObterCursosDisponiveisAsync());
    [AllowAnonymous, HttpPost("solicitacoes")]
    public async Task<IActionResult> RequestEnrollment(SolicitarMatriculaDto request)
    {
        int? userId = int.TryParse(User.FindFirst("sub")?.Value, out var id) ? id : null;
        var requestId = await _useCase.SolicitarMatriculaAsync(request, userId);
        return Accepted(new { id = requestId, status = "Pendente" });
    }
}
