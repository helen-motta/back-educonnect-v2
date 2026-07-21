using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Academico.Application.DTOs;
using Modules.Academico.Application.UseCases;

namespace Modules.Academico.Api.Controllers;

[ApiController, Authorize, Route("api/configuracoes-portal")]
public sealed class ConfiguracoesPortalController : ControllerBase
{
    private readonly PortalUseCase _useCase;
    public ConfiguracoesPortalController(PortalUseCase useCase) => _useCase = useCase;
    [HttpGet] public async Task<IActionResult> Get() => Ok(await _useCase.ObterConfiguracaoAsync());
    [HttpPut] public async Task<IActionResult> Put(PortalConfiguracaoDto request) => Ok(await _useCase.SalvarConfiguracaoAsync(request));
}
