using Modules.Academico.Domain.Entities;
using Modules.Academico.Application.DTOs;

namespace Modules.Academico.Domain.Interfaces
{
    public interface ITurmasRepository
    {
        Task<IEnumerable<Turma>> Execute(TurmaListagemDto filtro);
        Task<IEnumerable<TurmaListaDTO>> GetTurmaById(int id);
        Task<IEnumerable<HorarioAlunoDTO>> GetHorariosPorAluno(int alunoId);
        Task<IEnumerable<NotasFrequenciaDto>> GetNotasEFrequenciaPorAluno(int alunoId);
        Task<List<int>> GetDisciplinasPorAluno(int alunoId);
        Task<List<TurmaListaDTO>> GetTurmasPorProfessor(int professorId);
    }
}
