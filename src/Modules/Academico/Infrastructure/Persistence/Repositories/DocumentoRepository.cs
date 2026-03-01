using Microsoft.EntityFrameworkCore;
using Modules.Academico.Domain.Interfaces;
using Shared.Infrastructure;

public class DocumentoRepository : IDocumentoRepository
{
    private readonly AppDbContext _context;

    public DocumentoRepository(AppDbContext context)
    {
        _context = context;
    }

}
