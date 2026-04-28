using Modules.Academico.Domain.Entities;

namespace Modules.Academico.Domain.Interfaces
{
    public interface IProfessorLancamentosRepository
    {
        Task<bool> ProfessorResponsavelPelaTurmaAsync(int professorId, int turmaId);
        Task<int?> ObterTurmaIdPorMatriculaAsync(int matriculaId);
        Task<bool> AvaliacaoPertenceATurmaAsync(int avaliacaoId, int turmaId);
        Task<List<AvaliacaoProfessor>> ObterAvaliacoesPorTurmaAsync(int turmaId);
        Task<List<FrequenciaProfessor>> ObterFrequenciasPorMatriculaAsync(int matriculaId);
        Task<List<NotaProfessor>> ObterNotasPorMatriculaAsync(int matriculaId);
        Task<AvaliacaoProfessor> CriarAvaliacaoAsync(AvaliacaoProfessor avaliacao);
        Task<FrequenciaProfessor> LancarFrequenciaAsync(FrequenciaProfessor frequencia);
        Task<NotaProfessor> LancarNotaAsync(NotaProfessor nota);
    }
}
