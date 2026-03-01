using Modules.Academico.Domain.Entities;

namespace Modules.Academico.Domain.Interfaces
{
    public interface IDisciplinasRepository
    {
        Task<List<Disciplina>> BuscarPorCursoId(int cursoId);
        Task<Disciplina> AdicionarAsync(Disciplina disciplina);
    }
}
