namespace Modules.Autenticacao.Application.DTOs
{
    public class ResetSenhaDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NovaSenha { get; set; }
    }
}
