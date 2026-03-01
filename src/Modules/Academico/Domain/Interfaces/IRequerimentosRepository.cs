using System.Threading.Tasks;
using Modules.Academico.Application.DTOs;

namespace Modules.Academico.Domain.Interfaces
{
    public interface IRequerimentosRepository
    {
        Task<Requerimentos> AdicionarAsync(Requerimentos requerimento);
        Task<List<Requerimentos>> BuscarPorUsuarioAsync(int idUsuario);
        Task<Requerimentos?> BuscarPorIdAsync(int id);
        Task AtualizarAsync(Requerimentos requerimento);
        Task<(IEnumerable<Requerimentos> requerimentos, int total)> ListarRequerimentosPaginados(PaginacaoRequerimentosDto filtro);
    }
}
