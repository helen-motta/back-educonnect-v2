using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure;
using src.Modules.Academico.Domain.Entities;

namespace Modules.Academico.Domain.Interfaces
{
    public class CursosRepository : ICursosRepository
    {
        private readonly AppDbContext _context;

        public CursosRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Curso?> BuscarPorIdAsync(int id)
        {
            return await _context.Cursos
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<(IEnumerable<Curso> cursos, int total)> ListarCursosPaginados(PaginacaoCursosDto filtro)
        {
            var query = _context.Cursos
                .Include(c => c.Coordenador)
                .Include(c => c.Disciplinas)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro.Nome))
                query = query.Where(c => c.Nome.Contains(filtro.Nome));

            if (!string.IsNullOrWhiteSpace(filtro.Codigo))
                query = query.Where(c => c.Codigo.Contains(filtro.Codigo));

            if (filtro.Ativo) query = query.Where(c => c.Ativo);
            else if (!filtro.Ativo) query = query.Where(c => !c.Ativo);
            var total = await query.CountAsync();

            var cursos = await query
                .OrderBy(c => c.Nome)
                .Skip((filtro.PaginaNumero - 1) * filtro.PaginaTamanho)
                .Take(filtro.PaginaTamanho)
                .ToListAsync();

            return (cursos, total);
        }

        public async Task<Curso> AdicionarAsync(Curso curso)
        {
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();
            return curso;
        }
    }
}