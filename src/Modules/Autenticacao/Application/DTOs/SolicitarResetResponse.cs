namespace Modules.Autenticacao.Application.DTOs
{
    public class SolicitarResetResponse
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public string? Token { get; set; }
    }
}
