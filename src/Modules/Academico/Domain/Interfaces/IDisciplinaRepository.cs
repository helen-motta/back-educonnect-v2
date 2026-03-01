using Modules.Academico.Domain.Entities;

namespace Modules.Academico.Domain.Interfaces
{
    public interface IDisciplinaRepository
    {
        Task<Disciplina?> BuscarPorIdAsync(int disciplinaId);
        Task<IEnumerable<Disciplina>> BuscarTodasAsync();
    }
}
