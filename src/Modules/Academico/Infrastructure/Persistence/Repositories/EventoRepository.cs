// Infrastructure/Persistence/Repositories/EventoRepository.cs
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure;

public class EventoRepository : IEventoRepository
{
    private readonly AppDbContext _context;

    public EventoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Eventos>> ObterEventosVisiveisAsync(List<int>? disciplinasIds = null)
    {
        var query = _context.Eventos.AsQueryable();

        if (disciplinasIds != null && disciplinasIds.Any())
        {
            query = query.Where(e => 
                e.Tipo == TipoEvento.Seminario || 
                e.Tipo == TipoEvento.Workshop || 
                (e.Tipo == TipoEvento.Disciplina && e.DisciplinaId.HasValue && disciplinasIds.Contains(e.DisciplinaId.Value))
            );
        }

        return await query.ToListAsync();
    }

    public async Task AdicionarAsync(Eventos evento)
    {
        await _context.Eventos.AddAsync(evento);
        await _context.SaveChangesAsync();
    }
}