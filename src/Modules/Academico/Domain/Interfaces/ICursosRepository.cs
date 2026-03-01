using src.Modules.Academico.Domain.Entities;

namespace Modules.Academico.Domain.Interfaces
{
    public interface ICursosRepository
    {
        Task<Curso?> BuscarPorIdAsync(int id);
        Task<(IEnumerable<Curso> cursos, int total)> ListarCursosPaginados(PaginacaoCursosDto filtro);
        Task<Curso> AdicionarAsync(Curso curso);
    }
}
