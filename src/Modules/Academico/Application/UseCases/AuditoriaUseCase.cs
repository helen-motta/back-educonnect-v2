using System.Text.Json;
using Modules.Academico.Application.DTOs;
using Modules.Academico.Domain.Interfaces;
using Shared.Domain.Entities;

namespace Modules.Academico.Application.UseCases;

public class AuditoriaUseCase
{
    private readonly IAuditoriaRepository _auditoriaRepository;

    public AuditoriaUseCase(IAuditoriaRepository auditoriaRepository)
    {
        _auditoriaRepository = auditoriaRepository;
    }

    public async Task RegistrarAsync(RegistrarAuditoriaRequestDto request)
    {
        var registro = new Auditoria
        {
            TabelaNome = request.TabelaNome,
            EntidadeId = request.EntidadeId,
            Operacao = request.Operacao,
            DadosAnterior = SerializarDados(request.DadosAnterior),
            DadosAtual = SerializarDados(request.DadosAtual),
            UsuarioId = request.UsuarioId,
            EnderecoIp = request.EnderecoIp,
            UserAgent = request.UserAgent,
            DataHora = DateTimeOffset.UtcNow
        };

        await _auditoriaRepository.AdicionarAsync(registro);
    }

    private static string? SerializarDados(object? dados)
    {
        if (dados is null)
            return null;

        if (dados is string valorString)
            return JsonSerializer.Serialize(valorString);

        return JsonSerializer.Serialize(dados);
    }
}
