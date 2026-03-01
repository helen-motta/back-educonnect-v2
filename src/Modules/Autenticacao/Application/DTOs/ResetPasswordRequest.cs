namespace Modules.Autenticacao.Application.DTOs
{
    public class ResetPasswordRequest
    {
        public string Email { get; set; }
        public string NovaSenha { get; set; }
    }
}
