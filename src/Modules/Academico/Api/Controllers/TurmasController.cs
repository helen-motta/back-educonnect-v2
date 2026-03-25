using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modules.Academico.Application.DTOs;
using Modules.Academico.Application.UseCases;
using Shared.Infrastructure;

[ApiController]
[Route("api/[controller]")]
public class TurmasController : ControllerBase
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

        await _auditoriaUseCase.RegistrarAsync(new RegistrarAuditoriaRequestDto
        {
            TabelaNome = "turmas",
            EntidadeId = turmaId.ToString(),
            Operacao = "SELECT",
            DadosAtual = new { turmaId },
            UsuarioId = ObterUsuarioId(),
            EnderecoIp = ObterEnderecoIp(),
            UserAgent = ObterUserAgent()
        });

        return Ok(response);
    }

    [HttpGet("aluno/{alunoId}/horarios")]
    public async Task<IActionResult> GetHorariosPorAluno(int alunoId)
    {
        var response = await _turmasUseCase.GetHorariosPorAluno(alunoId);

        await _auditoriaUseCase.RegistrarAsync(new RegistrarAuditoriaRequestDto
        {
            TabelaNome = "inscricoes_turmas",
            EntidadeId = alunoId.ToString(),
            Operacao = "SELECT",
            DadosAtual = new { alunoId, endpoint = "horarios" },
            UsuarioId = ObterUsuarioId(),
            EnderecoIp = ObterEnderecoIp(),
            UserAgent = ObterUserAgent()
        });

        return Ok(response);
    }

    [HttpGet("professor")]
    public async Task<IActionResult> GetTurmasPorProfessor()
    {
        var professorId = int.Parse(User.FindFirst("sub")?.Value ?? "0");

        var response = await _turmasUseCase.GetTurmasPorProfessor(professorId);

        await _auditoriaUseCase.RegistrarAsync(new RegistrarAuditoriaRequestDto
        {
            TabelaNome = "turmas",
            EntidadeId = professorId.ToString(),
            Operacao = "SELECT",
            DadosAtual = new { professorId, endpoint = "professor" },
            UsuarioId = ObterUsuarioId(),
            EnderecoIp = ObterEnderecoIp(),
            UserAgent = ObterUserAgent()
        });

        return Ok(response);
    }

    private string ObterUsuarioId()
    {
        return User.FindFirst("sub")?.Value ?? "anonimo";
    }

    private string? ObterEnderecoIp()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }

    private string? ObterUserAgent()
    {
        return Request.Headers.UserAgent.ToString();
    }
}