using Microsoft.EntityFrameworkCore;
using Modules.Academico.Application.DTOs;
using Modules.Academico.Domain.Entities;
using Modules.Academico.Domain.Interfaces;
using Shared.Infrastructure;

namespace Modules.Academico.Infrastructure.Persistence.Repositories;

public sealed class TurmasRepository : ITurmasRepository
{
    private readonly AppDbContext _context;
    public TurmasRepository(AppDbContext context) => _context = context;

    public async Task<List<TurmaListaDTO>> Execute(TurmaListagemDto filtro)
    {
        var query = BaseQuery();
        if (!string.IsNullOrWhiteSpace(filtro.NomeTurma)) query = query.Where(x => x.NomeTurma.Contains(filtro.NomeTurma));
        if (filtro.DisciplinaId.HasValue) query = query.Where(x => x.DisciplinaId == filtro.DisciplinaId);
        if (filtro.ProfessorId.HasValue) query = query.Where(x => x.ProfessorId == filtro.ProfessorId);
        return (await query.OrderBy(x => x.NomeTurma).ToListAsync()).Select(MapList).ToList();
    }

    public async Task<TurmaDetalheDto?> GetTurmaById(int id)
    {
        var item = await BaseQuery().SingleOrDefaultAsync(x => x.Id == id);
        if (item is null) return null;
        var registrations = await _context.Matriculas.AsNoTracking().Where(x => x.TurmaId == id).ToDictionaryAsync(x => x.AlunoId, x => x.Id);
        var evaluations = await _context.AvaliacoesProfessor.AsNoTracking().Where(x => x.IdTurma == id).OrderBy(x => x.DataPrevista).ToListAsync();
        return new TurmaDetalheDto(item.Id, item.NomeTurma, item.Sala, item.Disciplina.Nome,
            item.InscricoesTurmas.Select(x => new AlunoTurmaDto(x.AlunoId, registrations.GetValueOrDefault(x.AlunoId), x.Aluno.Nome, x.Aluno.Matricula, x.P1, x.P2, x.Trabalho)).ToList(),
            evaluations.Select(x => new AvaliacaoTurmaDto(x.Id, x.Nome, x.Peso, x.DataPrevista)).ToList());
    }

    public async Task<IEnumerable<HorarioAlunoDTO>> GetHorariosPorAluno(int alunoId) =>
        await _context.TurmaSlots.AsNoTracking()
            .Where(slot => slot.Turma.InscricoesTurmas.Any(x => x.AlunoId == alunoId))
            .Select(slot => new HorarioAlunoDTO
            {
                CodigoSlot = slot.CodigoSlot, DiaSemana = slot.DiaSemana, Disciplina = slot.Turma.Disciplina.Nome,
                Professor = slot.Turma.Professor.Nome, Sala = slot.Turma.Sala
            }).ToListAsync();

    public async Task<IEnumerable<NotasFrequenciaDto>> GetNotasEFrequenciaPorAluno(int alunoId) =>
        await _context.InscricoesTurmas.AsNoTracking().Where(x => x.AlunoId == alunoId).Select(x => new NotasFrequenciaDto
        {
            TurmaId = x.TurmaId, Disciplina = x.Turma.Disciplina.Nome, Professor = x.Turma.Professor.Nome,
            P1 = x.P1 ?? 0, P2 = x.P2 ?? 0, Trabalho = x.Trabalho ?? 0, NotaFinal = x.NotaFinal, FrequenciaPercentual = x.Frequencia ?? 0
        }).ToListAsync();

    public Task<List<int>> GetDisciplinasPorAluno(int alunoId) => _context.InscricoesTurmas.AsNoTracking()
        .Where(x => x.AlunoId == alunoId).Select(x => x.Turma.DisciplinaId).Distinct().ToListAsync();

    public async Task<List<TurmaListaDTO>> GetTurmasPorProfessor(int professorId) =>
        (await BaseQuery().Where(x => x.ProfessorId == professorId).OrderBy(x => x.NomeTurma).ToListAsync()).Select(MapList).ToList();

    public async Task<Turma> CriarAsync(SalvarTurmaDto request)
    {
        var item = new Turma { NomeTurma = request.NomeTurma.Trim(), DisciplinaId = request.DisciplinaId!.Value, ProfessorId = request.ProfessorId!.Value, Sala = request.Sala?.Trim() ?? "A definir", Vagas = request.Vagas ?? 30 };
        _context.Turmas.Add(item); await _context.SaveChangesAsync(); return item;
    }

    public async Task<Turma?> AtualizarAsync(int id, SalvarTurmaDto request)
    {
        var item = await _context.Turmas.FindAsync(id); if (item is null) return null;
        item.NomeTurma = request.NomeTurma.Trim(); item.DisciplinaId = request.DisciplinaId!.Value; item.ProfessorId = request.ProfessorId!.Value;
        item.Sala = request.Sala?.Trim() ?? item.Sala; item.Vagas = request.Vagas ?? item.Vagas;
        await _context.SaveChangesAsync(); return item;
    }

    public async Task<bool> RemoverAsync(int id)
    {
        var item = await _context.Turmas.FindAsync(id); if (item is null) return false;
        _context.Turmas.Remove(item); await _context.SaveChangesAsync(); return true;
    }

    private IQueryable<Turma> BaseQuery() => _context.Turmas.AsNoTracking().Include(x => x.Disciplina).Include(x => x.Professor)
        .Include(x => x.TurmaSlots).Include(x => x.InscricoesTurmas).ThenInclude(x => x.Aluno);

    private static TurmaListaDTO MapList(Turma x) => new()
    {
        Id = x.Id, NomeTurma = x.NomeTurma, DisciplinaId = x.DisciplinaId, DisciplinaNome = x.Disciplina.Nome,
        ProfessorId = x.ProfessorId, ProfessorNome = x.Professor.Nome, Sala = x.Sala, Vagas = x.Vagas,
        QuantidadeInscritos = x.InscricoesTurmas.Count,
        HorariosFormatados = x.TurmaSlots.OrderBy(slot => slot.DiaSemana).Select(slot => $"{slot.DiaSemanaNome}, {slot.Horario}").ToList()
    };
}
