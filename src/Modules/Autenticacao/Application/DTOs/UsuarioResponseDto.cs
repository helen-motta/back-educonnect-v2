namespace Modules.Autenticacao.Application.DTOs
{
    public class UsuarioResponseDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public int IdPerfil { get; set; }
        public string Papel { get; set; }
        public string Status { get; set; }
        public string Registro { get; set; }
        public string Cep { get; set; }
        public string Endereco { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Telefone { get; set; }
        public string Cpf { get; set; }
        public string Rg { get; set; }
    }
}
