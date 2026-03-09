using Microsoft.EntityFrameworkCore;
using Modules.Academico.Application.DTOs;
using Modules.Academico.Domain.Entities;
using Shared.Infrastructure;
using src.Modules.Academico.Domain.Entities;

namespace Modules.Academico.Domain.Interfaces
{
    public class TurmasRepository : ITurmasRepository
    {
        private readonly AppDbContext _context;

        public TurmasRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Turma>> Execute(TurmaListagemDto filtro)
        {
            var query = _context.Turmas
                .Include(t => t.Disciplina)
                .Include(t => t.Professor)
                .Include(t => t.TurmaSlots)
                .Include(t => t.InscricoesTurmas)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro.NomeTurma))
                query = query.Where(t => t.NomeTurma.Contains(filtro.NomeTurma));

            if (filtro.DisciplinaId != null)
                query = query.Where(t => t.DisciplinaId == filtro.DisciplinaId);

            if (filtro.ProfessorId != null)
                query = query.Where(t => t.ProfessorId == filtro.ProfessorId);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<TurmaListaDTO>> GetTurmaById(int id)
        {
            return await _context.Turmas
            .Where(t => t.Id == id)
            .Select(t => new TurmaListaDTO
            {
                Id = t.Id,
                NomeTurma = t.NomeTurma,
                DisciplinaNome = t.Disciplina.Nome,
                Vagas = t.Vagas,
                QuantidadeInscritos = t.InscricoesTurmas.Count()
            })
            .ToListAsync();
        }

        public async Task<IEnumerable<HorarioAlunoDTO>> GetHorariosPorAluno(int alunoId)
        {
            return await _context.InscricoesTurmas
                .Where(i => i.AlunoId == alunoId)
                .Select(i => new HorarioAlunoDTO
                {
                    CodigoSlot = i.Turma.TurmaSlots.CodigoSlot,
                    DiaSemana = i.Turma.TurmaSlots.DiaSemana,
                    Disciplina = i.Turma.Disciplina.Nome,
                    Professor = i.Turma.Professor.Nome,
                    Sala = i.Turma.Sala,
                    TurmaSlot = i.Turma.TurmaSlots
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<NotasFrequenciaDto>> GetNotasEFrequenciaPorAluno(int alunoId)
        {
            return await _context.InscricoesTurmas
                .Where(i => i.AlunoId == alunoId)
                .Include(i => i.Turma)
                    .ThenInclude(t => t.Disciplina)
                .Include(i => i.Turma)
                    .ThenInclude(t => t.Professor)
                .Select(i => new NotasFrequenciaDto
                {
                    TurmaId = i.TurmaId,
                    Disciplina = i.Turma.Disciplina.Nome,
                    Professor = i.Turma.Professor.Nome,
                    P1 = i.P1 ?? 0,
                    P2 = i.P2 ?? 0,
                    Trabalho = i.Trabalho ?? 0,
                    NotaFinal = i.NotaFinal,
                    FrequenciaPercentual = i.Frequencia ?? 0
                })
                .ToListAsync();
        }

        public async Task<List<int>> GetDisciplinasPorAluno(int alunoId)
        {
            return await _context.InscricoesTurmas
                .Where(i => i.AlunoId == alunoId)
                .Select(i => i.Turma.DisciplinaId)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<TurmaListaDTO>> GetTurmasPorProfessor(int professorId)
        {
            return await _context.Turmas
                .Where(t => t.ProfessorId == professorId)
                .Select(t => new TurmaListaDTO
                {
                    Id = t.Id,
                    NomeTurma = t.NomeTurma,
                    DisciplinaNome = t.Disciplina.Nome,
                    Vagas = t.Vagas,
                    QuantidadeInscritos = t.InscricoesTurmas.Count()
                })
                .ToListAsync();
        }
    }
}