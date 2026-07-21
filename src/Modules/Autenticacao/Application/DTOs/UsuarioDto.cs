namespace Modules.Autenticacao.Application.DTOs
{
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int IdPerfil { get; set; }
        public string Papel { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Registro { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Complemento { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string Rg { get; set; } = string.Empty;
        public string? FotoUrl { get; set; }
        public bool NotificarTarefas { get; set; }
        public bool NotificarAvisos { get; set; }
        public bool NotificarNotas { get; set; }
    }

    public sealed record PreferenciasNotificacaoDto(bool NotificarTarefas, bool NotificarAvisos, bool NotificarNotas);
}
