using Modules.Academico.Application.DTOs;
using Modules.Academico.Domain.Interfaces;

namespace Modules.Academico.Application.UseCases;

public sealed class TurmasUseCase
{
    private readonly ITurmasRepository _repository;
    public TurmasUseCase(ITurmasRepository repository) => _repository = repository;
    public Task<List<TurmaListaDTO>> Execute(TurmaListagemDto filtro) => _repository.Execute(filtro);
    public Task<TurmaDetalheDto?> GetTurmasById(int id) => _repository.GetTurmaById(id);
    public Task<IEnumerable<HorarioAlunoDTO>> GetHorariosPorAluno(int alunoId) => _repository.GetHorariosPorAluno(alunoId);
    public Task<IEnumerable<NotasFrequenciaDto>> GetNotasEFrequenciaPorAluno(int alunoId) => _repository.GetNotasEFrequenciaPorAluno(alunoId);
    public Task<List<TurmaListaDTO>> GetTurmasPorProfessor(int professorId) => _repository.GetTurmasPorProfessor(professorId);
    public async Task<TurmaListaDTO> CriarAsync(SalvarTurmaDto request) { Validate(request); var x = await _repository.CriarAsync(request); return (await _repository.Execute(new TurmaListagemDto { Id = x.Id })).Single(t => t.Id == x.Id); }
    public async Task AtualizarAsync(int id, SalvarTurmaDto request) { Validate(request); if (await _repository.AtualizarAsync(id, request) is null) throw new KeyNotFoundException("Turma não encontrada."); }
    public async Task RemoverAsync(int id) { if (!await _repository.RemoverAsync(id)) throw new KeyNotFoundException("Turma não encontrada."); }
    private static void Validate(SalvarTurmaDto request)
    {
        if (string.IsNullOrWhiteSpace(request.NomeTurma) || request.DisciplinaId is null or <= 0 || request.ProfessorId is null or <= 0)
            throw new ArgumentException("Nome, disciplina e professor são obrigatórios.");
    }
}
