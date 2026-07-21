using Modules.Academico.Application.DTOs;
using Modules.Academico.Application.UseCases;
using Modules.Academico.Domain.Interfaces;
using Shared.Domain.Entities;

namespace EduConnect.UnitTests;

public sealed class AuditoriaUseCaseTests
{
    [Fact]
    public async Task RegistrarAsync_PreservaContextoESerializaAlteracoes()
    {
        var repository = new FakeAuditoriaRepository();
        var useCase = new AuditoriaUseCase(repository);

        await useCase.RegistrarAsync(new RegistrarAuditoriaRequestDto
        {
            TabelaNome = "cursos",
            EntidadeId = "42",
            Operacao = "UPDATE",
            DadosAnterior = new { nome = "Anterior" },
            DadosAtual = new { nome = "Atual" },
            UsuarioId = "2",
            EnderecoIp = "127.0.0.1",
            UserAgent = "UnitTests"
        });

        var registro = Assert.IsType<Auditoria>(repository.Registro);
        Assert.NotEqual(Guid.Empty, registro.Id);
        Assert.Equal("cursos", registro.TabelaNome);
        Assert.Equal("42", registro.EntidadeId);
        Assert.Equal("UPDATE", registro.Operacao);
        Assert.Contains("Anterior", registro.DadosAnterior);
        Assert.Contains("Atual", registro.DadosAtual);
        Assert.Equal("2", registro.UsuarioId);
        Assert.Equal("127.0.0.1", registro.EnderecoIp);
        Assert.Equal("UnitTests", registro.UserAgent);
    }

    private sealed class FakeAuditoriaRepository : IAuditoriaRepository
    {
        public Auditoria? Registro { get; private set; }

        public Task AdicionarAsync(Auditoria auditoria)
        {
            Registro = auditoria;
            return Task.CompletedTask;
        }
    }
}
