namespace Modules.Autenticacao.Application.DTOs
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public UsuarioDto Usuario { get; set; }
        public bool NecessitaAceitarTermos { get; set; }
    }
}
