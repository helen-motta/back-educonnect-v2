using Modules.Academico.Domain.Entities;

namespace Modules.Academico.Domain.Interfaces
{
    public interface IAlunoRepository
    {
        Task<Aluno?> BuscarPorIdAsync(int alunoId);
        Task<IEnumerable<Aluno>> BuscarTodosAsync();
    }
}
