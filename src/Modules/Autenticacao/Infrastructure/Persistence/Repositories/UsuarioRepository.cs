using Microsoft.EntityFrameworkCore;
using Modules.Autenticacao.Application.DTOs;
using Modules.Autenticacao.Domain.Entities;
using Modules.Autenticacao.Domain.Enums;
using Modules.Autenticacao.Domain.Interfaces;
using Shared.Infrastructure;

namespace Modules.Autenticacao.Infrastructure.Persistence.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> BuscarPorEmailAsync(string email)
        {
            return await _context.Usuario
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<Usuario?> BuscarPorIdAsync(int id)
        {
            return await _context.Usuario
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task AtualizarAsync(Usuario usuario)
        {
            _context.Usuario.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task CriarAsync(Usuario usuario)
        {
            _context.Usuario.Add(usuario);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Usuario>> ObterUsuarios()
        {
            return await _context.Usuario.ToListAsync();
        }

        public async Task DeletarAsync(int id)
        {
            var usuario = await BuscarPorIdAsync(id);
            if (usuario != null)
            {
                _context.Usuario.Remove(usuario);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<(IEnumerable<Usuario> usuarios, int total)> ListarUsuariosPaginados(PaginacaoFiltroDto filtro)
        {
            var query = _context.Usuario.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro.Nome))
                query = query.Where(u => u.Nome.Contains(filtro.Nome));

            if (!string.IsNullOrWhiteSpace(filtro.Email))
                query = query.Where(u => u.Email.Contains(filtro.Email));

            if (!string.IsNullOrWhiteSpace(filtro.Registro))
                query = query.Where(u => u.Registro.Contains(filtro.Registro));
            
            if (filtro.Status == "Ativo") query = query.Where(u => u.Ativo);
            else if (filtro.Status == "Inativo") query = query.Where(u => !u.Ativo);

            if (filtro.IdPerfil is not null)
                query = query.Where(u => u.IdPerfil == filtro.IdPerfil);

            var total = await query.CountAsync();

            var usuarios = await query
                .Skip((filtro.PaginaNumero - 1) * filtro.PaginaTamanho)
                .Take(filtro.PaginaTamanho)
                .ToListAsync();

            return (usuarios, total);
        }
        public async Task<Usuario?> BuscarPorRegistroAsync(string registro)
        {
            return await _context.Usuario
                .FirstOrDefaultAsync(u => u.Registro == registro);
        }

        public async Task<string?> ObterUltimoRegistroPorPrefixoAsync(string prefixo)
        {
            return await _context.Usuario
                .Where(u => u.Registro.StartsWith(prefixo))
                .OrderByDescending(u => u.Registro)
                .Select(u => u.Registro)
                .FirstOrDefaultAsync();
        }
    }
}
