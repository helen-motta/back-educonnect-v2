using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Autenticacao.Application.DTOs;
using Modules.Autenticacao.Application.UseCases;
using Shared.Domain.Interfaces;

namespace Modules.Autenticacao.Api.Controllers;

[ApiController, Authorize, Route("api/perfil")]
public sealed class PerfilController : ControllerBase
{
    private readonly UsuarioUseCase _useCase;
    private readonly IFileStorageService _storage;
    public PerfilController(UsuarioUseCase useCase, IFileStorageService storage) { _useCase = useCase; _storage = storage; }

    [HttpGet]
    public async Task<IActionResult> GetPerfil() => Ok(await _useCase.ObterUsuarioPorIdAsync(UserId()));

    [HttpPost("foto")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<IActionResult> UploadFoto([FromForm] IFormFile arquivo, CancellationToken cancellationToken)
    {
        if (arquivo.Length == 0) return BadRequest(new { message = "Selecione uma imagem." });
        await using var stream = arquivo.OpenReadStream();
        var url = await _storage.UploadAsync(stream, arquivo.ContentType, arquivo.FileName, cancellationToken);
        await _useCase.AtualizarFotoAsync(UserId(), url);
        return Ok(new { fotoUrl = url });
    }

    [HttpPut("preferencias")]
    public async Task<IActionResult> PutPreferencias(PreferenciasNotificacaoDto request) =>
        Ok(await _useCase.AtualizarPreferenciasAsync(UserId(), request));

    private int UserId() => int.TryParse(User.FindFirst("sub")?.Value, out var id) ? id : throw new UnauthorizedAccessException();
}
