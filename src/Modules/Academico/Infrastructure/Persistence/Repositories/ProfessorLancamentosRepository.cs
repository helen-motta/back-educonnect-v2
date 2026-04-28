using Microsoft.EntityFrameworkCore;
using Modules.Academico.Domain.Entities;
using Modules.Academico.Domain.Interfaces;
using Shared.Infrastructure;

namespace Modules.Academico.Infrastructure.Persistence.Repositories
{
    public class ProfessorLancamentosRepository : IProfessorLancamentosRepository
    {
        private readonly AppDbContext _context;

        public ProfessorLancamentosRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ProfessorResponsavelPelaTurmaAsync(int professorId, int turmaId)
        {
            return await _context.Turmas
                .AsNoTracking()
                .AnyAsync(t => t.Id == turmaId && t.ProfessorId == professorId);
        }

        public async Task<int?> ObterTurmaIdPorMatriculaAsync(int matriculaId)
        {
            return await _context.Matriculas
                .AsNoTracking()
                .Where(m => m.Id == matriculaId)
                .Select(m => (int?)m.TurmaId)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> AvaliacaoPertenceATurmaAsync(int avaliacaoId, int turmaId)
        {
            return await _context.AvaliacoesProfessor
                .AsNoTracking()
                .AnyAsync(a => a.Id == avaliacaoId && a.IdTurma == turmaId);
        }

        public async Task<List<AvaliacaoProfessor>> ObterAvaliacoesPorTurmaAsync(int turmaId)
        {
            return await _context.AvaliacoesProfessor
                .AsNoTracking()
                .Where(a => a.IdTurma == turmaId)
                .OrderBy(a => a.DataPrevista)
                .ThenBy(a => a.Nome)
                .ToListAsync();
        }

        public async Task<List<FrequenciaProfessor>> ObterFrequenciasPorMatriculaAsync(int matriculaId)
        {
            return await _context.FrequenciasProfessor
                .AsNoTracking()
                .Where(f => f.IdMatricula == matriculaId)
                .OrderByDescending(f => f.DataAula)
                .ToListAsync();
        }

        public async Task<List<NotaProfessor>> ObterNotasPorMatriculaAsync(int matriculaId)
        {
            return await _context.NotasProfessor
                .AsNoTracking()
                .Where(n => n.IdMatricula == matriculaId)
                .OrderByDescending(n => n.Id)
                .ToListAsync();
        }

        public async Task<AvaliacaoProfessor> CriarAvaliacaoAsync(AvaliacaoProfessor avaliacao)
        {
            _context.AvaliacoesProfessor.Add(avaliacao);
            await _context.SaveChangesAsync();
            return avaliacao;
        }

        public async Task<FrequenciaProfessor> LancarFrequenciaAsync(FrequenciaProfessor frequencia)
        {
            _context.FrequenciasProfessor.Add(frequencia);
            await _context.SaveChangesAsync();
            return frequencia;
        }

        public async Task<NotaProfessor> LancarNotaAsync(NotaProfessor nota)
        {
            _context.NotasProfessor.Add(nota);
            await _context.SaveChangesAsync();
            return nota;
        }
    }
}
