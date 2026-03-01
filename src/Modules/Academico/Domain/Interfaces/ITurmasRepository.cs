using Modules.Academico.Domain.Entities;

namespace Modules.Academico.Domain.Interfaces
{
    public interface ITurmasRepository
    {
        Task<IEnumerable<Turma>> Execute(TurmaListagemDto filtro);
    }
}
