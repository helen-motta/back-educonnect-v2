using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Academico.Application.UseCases;

namespace Modules.Academico.Api.Controllers;

[ApiController, Authorize, Route("api/dashboard")]
public sealed class PortalDashboardController : ControllerBase
{
    private readonly PortalUseCase _useCase;
    public PortalDashboardController(PortalUseCase useCase) => _useCase = useCase;
    [HttpGet("professor")] public async Task<IActionResult> Professor() => Ok(await _useCase.ObterDashboardProfessorAsync(UserId()));
    [HttpGet("coordenador")] public async Task<IActionResult> Coordenador() => Ok(await _useCase.ObterDashboardCoordenadorAsync());
    private int UserId() => int.TryParse(User.FindFirst("sub")?.Value, out var id) ? id : throw new UnauthorizedAccessException();
}
