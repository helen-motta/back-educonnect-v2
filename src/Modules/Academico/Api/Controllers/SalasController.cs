using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Academico.Application.UseCases;

namespace Modules.Academico.Api.Controllers;

[ApiController, Authorize, Route("api/salas")]
public sealed class SalasController : ControllerBase
{
    private readonly PortalUseCase _useCase;
    public SalasController(PortalUseCase useCase) => _useCase = useCase;
    [HttpGet] public async Task<IActionResult> Get() => Ok(await _useCase.ObterSalasAsync());
}
