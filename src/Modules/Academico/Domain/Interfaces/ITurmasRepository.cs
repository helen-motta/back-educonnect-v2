using Modules.Academico.Application.DTOs;
using Modules.Academico.Domain.Entities;

namespace Modules.Academico.Domain.Interfaces;

public interface ITurmasRepository
{
    Task<List<TurmaListaDTO>> Execute(TurmaListagemDto filtro);
    Task<TurmaDetalheDto?> GetTurmaById(int id);
    Task<IEnumerable<HorarioAlunoDTO>> GetHorariosPorAluno(int alunoId);
    Task<IEnumerable<NotasFrequenciaDto>> GetNotasEFrequenciaPorAluno(int alunoId);
    Task<List<int>> GetDisciplinasPorAluno(int alunoId);
    Task<List<TurmaListaDTO>> GetTurmasPorProfessor(int professorId);
    Task<Turma> CriarAsync(SalvarTurmaDto request);
    Task<Turma?> AtualizarAsync(int id, SalvarTurmaDto request);
    Task<bool> RemoverAsync(int id);
}
