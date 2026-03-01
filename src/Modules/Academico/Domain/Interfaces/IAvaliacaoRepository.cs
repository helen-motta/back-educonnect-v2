using Modules.Academico.Domain.Entities;

namespace Modules.Academico.Domain.Interfaces
{
    public interface IAvaliacaoRepository
    {
        Task<IEnumerable<Avaliacao>> BuscarPorMatriculaAsync(int matriculaId);
        Task<Avaliacao?> BuscarPorIdAsync(int avaliacaoId);
    }
}
