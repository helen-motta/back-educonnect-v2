using Modules.Autenticacao.Application.DTOs;
using Modules.Autenticacao.Domain.Entities;

namespace Modules.Autenticacao.Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> BuscarPorEmailAsync(string email);
        Task<Usuario?> BuscarPorIdAsync(int id);
        Task AtualizarAsync(Usuario usuario);
        Task CriarAsync(Usuario usuario);
        Task<List<Usuario>> ObterUsuarios();
        Task DeletarAsync(int id);
        Task<Usuario?> BuscarPorRegistroAsync(string registro);
        Task<(IEnumerable<Usuario> usuarios, int total)>ListarUsuariosPaginados(PaginacaoFiltroDto filtro);
        Task<string?> ObterUltimoRegistroPorPrefixoAsync(string prefixo);
    }
}
