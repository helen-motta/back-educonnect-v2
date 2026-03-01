using Modules.Academico.Domain.Entities;

namespace Modules.Academico.Domain.Interfaces
{
    public interface IFrequenciaRepository
    {
        Task<Frequencia?> BuscarPorMatriculaAsync(int matriculaId);
        Task<Frequencia?> BuscarPorIdAsync(int frequenciaId);
    }
}
