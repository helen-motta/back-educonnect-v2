using Microsoft.EntityFrameworkCore;
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
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro.NomeTurma))
                query = query.Where(t => t.NomeTurma.Contains(filtro.NomeTurma));

            if (filtro.DisciplinaId != null)
                query = query.Where(t => t.DisciplinaId == filtro.DisciplinaId);

            if (filtro.ProfessorId != null)
                query = query.Where(t => t.ProfessorId == filtro.ProfessorId);

            return await query.ToListAsync();
        }
    }
}