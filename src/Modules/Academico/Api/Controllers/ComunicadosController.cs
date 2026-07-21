using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Academico.Application.DTOs;
using Modules.Academico.Application.UseCases;

namespace Modules.Academico.Api.Controllers;

[ApiController, Authorize, Route("api/comunicados")]
public sealed class ComunicadosController : ControllerBase
{
    private readonly PortalUseCase _useCase;
    public ComunicadosController(PortalUseCase useCase) => _useCase = useCase;
    [HttpGet] public async Task<IActionResult> Get() => Ok(await _useCase.ObterComunicadosAsync(UserId()));
    [HttpPost] public async Task<IActionResult> Post(CriarComunicadoDto request) => Ok(await _useCase.CriarComunicadoAsync(UserId(), request));
    private int UserId() => int.TryParse(User.FindFirst("sub")?.Value, out var id) ? id : throw new UnauthorizedAccessException();
}
