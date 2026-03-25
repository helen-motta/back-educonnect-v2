using Modules.Academico.Domain.Interfaces;
using Shared.Domain.Entities;
using Shared.Infrastructure;

namespace Modules.Academico.Infrastructure.Persistence.Repositories;

public class AuditoriaRepository : IAuditoriaRepository
{
    private readonly AppDbContext _context;

    public AuditoriaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AdicionarAsync(Auditoria auditoria)
    {
        await _context.Auditorias.AddAsync(auditoria);
        await _context.SaveChangesAsync();
    }
}
