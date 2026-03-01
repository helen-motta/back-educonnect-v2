using Modules.Autenticacao.Domain.Entities;

namespace Modules.Autenticacao.Domain.Interfaces
{
    public interface ITokenService
    {
        string GerarToken(Usuario usuario);
        bool ValidarToken(string token);
    }
}
