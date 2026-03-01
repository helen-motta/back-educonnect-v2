using Microsoft.EntityFrameworkCore;
using Modules.Academico.Domain.Entities;
using Shared.Infrastructure;
using src.Modules.Academico.Domain.Entities;

namespace Modules.Academico.Domain.Interfaces
{
    public class DisciplinasRepository : IDisciplinasRepository
    {
        private readonly AppDbContext _context;

        public DisciplinasRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Disciplina>> BuscarPorCursoId(int cursoId)
        {
            return _context.Disciplinas
                .Where(d => d.IdCurso == cursoId)
                .ToList();
        }

        public async Task<Disciplina> AdicionarAsync(Disciplina disciplina)
        {
            _context.Disciplinas.Add(disciplina);
            await _context.SaveChangesAsync();
            return disciplina;
        }
    }
}