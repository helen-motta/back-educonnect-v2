using Modules.Autenticacao.Application.DTOs;
using Modules.Autenticacao.Domain.Entities;
using Modules.Autenticacao.Domain.Interfaces;

namespace EduConnect.UnitTests;

internal sealed class FakeUsuarioRepository : IUsuarioRepository
{
    private readonly Usuario? _usuario;

    public FakeUsuarioRepository(Usuario? usuario = null) => _usuario = usuario;

    public int BuscasPorEmail { get; private set; }
    public int Atualizacoes { get; private set; }

    public Task<Usuario?> BuscarPorEmailAsync(string email)
    {
        BuscasPorEmail++;
        return Task.FromResult(_usuario?.Email == email ? _usuario : null);
    }

    public Task<Usuario?> BuscarPorIdAsync(int id) => Task.FromResult(_usuario?.Id == id ? _usuario : null);

    public Task AtualizarAsync(Usuario usuario)
    {
        Atualizacoes++;
        return Task.CompletedTask;
    }

    public Task CriarAsync(Usuario usuario) => Task.CompletedTask;
    public Task<List<Usuario>> ObterUsuarios() => Task.FromResult(_usuario is null ? new List<Usuario>() : new List<Usuario> { _usuario });
    public Task DeletarAsync(int id) => Task.CompletedTask;
    public Task<Usuario?> BuscarPorRegistroAsync(string registro) => Task.FromResult(_usuario?.Registro == registro ? _usuario : null);
    public Task<(IEnumerable<Usuario> usuarios, int total)> ListarUsuariosPaginados(PaginacaoFiltroDto filtro)
    {
        IEnumerable<Usuario> usuarios = _usuario is null ? [] : [_usuario];
        return Task.FromResult((usuarios, usuarios.Count()));
    }

    public Task<string?> ObterUltimoRegistroPorPrefixoAsync(string prefixo) => Task.FromResult<string?>(null);
}
