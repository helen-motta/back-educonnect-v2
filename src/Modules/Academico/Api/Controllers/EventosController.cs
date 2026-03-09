// Api/Controllers/EventosController.cs
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class EventosController : ControllerBase
{
    private readonly ListarEventosUseCase _listarEventosUseCase;
    private readonly ListarEventosUseCase _criarEventoUseCase; // Use Case de criação (a fazer)

    public EventosController(ListarEventosUseCase listarEventosUseCase, ListarEventosUseCase criarEventoUseCase)
    {
        _listarEventosUseCase = listarEventosUseCase;
        _criarEventoUseCase = criarEventoUseCase;
    }

    [HttpGet]
    public async Task<IActionResult> GetEventos()
    {
        var usuarioId = int.Parse(User.FindFirst("sub")?.Value);

        var eventos = await _listarEventosUseCase.ExecutarAsync(usuarioId);
        return Ok(eventos);
    }

    [HttpPost]
    public async Task<IActionResult> CriarEvento([FromBody] CriarEventoRequestDto request)
    {
        var professorId = int.Parse(User.FindFirst("sub")?.Value);
        
        await _criarEventoUseCase.AdicionarEventoAsync(professorId, request);
        return Created("", new { message = "Evento criado com sucesso" });
    }
}