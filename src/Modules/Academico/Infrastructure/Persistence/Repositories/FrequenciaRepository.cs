using Modules.Academico.Domain.Entities;
using Modules.Academico.Domain.Interfaces;

namespace Modules.Academico.Infrastructure.Persistence.Repositories
{
    public class FrequenciaRepository : IFrequenciaRepository
    {
        private static readonly List<Frequencia> _frequencias = new()
        {
            new Frequencia(1, 1, 45, 60),   // Matricula 1: 75%
            new Frequencia(2, 2, 57, 60),   // Matricula 2: 95%
            new Frequencia(3, 3, 40, 60),   // Matricula 3: 66.67% (reprovado)
            new Frequencia(4, 4, 54, 60),   // Matricula 4: 90%
            new Frequencia(5, 5, 48, 60),   // Matricula 5: 80%
            new Frequencia(6, 6, 52, 60),   // Matricula 6: 86.67%
            new Frequencia(7, 7, 58, 60)    // Matricula 7: 96.67%
        };

        public Task<Frequencia?> BuscarPorMatriculaAsync(int matriculaId)
        {
            var frequencia = _frequencias.FirstOrDefault(f => f.MatriculaId == matriculaId);
            return Task.FromResult(frequencia);
        }

        public Task<Frequencia?> BuscarPorIdAsync(int frequenciaId)
        {
            var frequencia = _frequencias.FirstOrDefault(f => f.Id == frequenciaId);
            return Task.FromResult(frequencia);
        }
    }
}
