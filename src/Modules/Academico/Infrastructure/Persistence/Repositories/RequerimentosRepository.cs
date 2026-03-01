using Microsoft.EntityFrameworkCore;
using Modules.Academico.Domain.Interfaces;
using Shared.Infrastructure;

public class RequerimentosRepository : IRequerimentosRepository
{
    private readonly AppDbContext _context;

    public RequerimentosRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Requerimentos> AdicionarAsync(Requerimentos requerimento)
    {
        _context.Requerimentos.Add(requerimento);
        await _context.SaveChangesAsync();
        return requerimento;
    }

    public async Task<List<Requerimentos>> BuscarPorUsuarioAsync(int idUsuario)
    {
        return await _context.Requerimentos
            .Where(r => r.IdUsuario == idUsuario)
            .OrderByDescending(r => r.DataAbertura)
            .ToListAsync();
    }

    public async Task<Requerimentos?> BuscarPorIdAsync(int id)
    {
        return await _context.Requerimentos
            .Include(r => r.Usuario)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task AtualizarAsync(Requerimentos requerimento)
    {
        _context.Requerimentos.Update(requerimento);
        await _context.SaveChangesAsync();
    }

    public async Task<(IEnumerable<Requerimentos> requerimentos, int total)> ListarRequerimentosPaginados(Modules.Academico.Application.DTOs.PaginacaoRequerimentosDto filtro)
    {
        var query = _context.Requerimentos
            .Include(r => r.Usuario)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filtro.Status))
            query = query.Where(r => r.Status.Contains(filtro.Status));

        if (!string.IsNullOrWhiteSpace(filtro.Tipo))
            query = query.Where(r => r.TipoSolicitacao.Contains(filtro.Tipo));

        var total = await query.CountAsync();

        var requerimentos = await query
            .OrderByDescending(r => r.DataAbertura)
            .Skip((filtro.PaginaNumero - 1) * filtro.PaginaTamanho)
            .Take(filtro.PaginaTamanho)
            .ToListAsync();

        return (requerimentos, total);
    }
}
