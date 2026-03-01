namespace Modules.Academico.Application.DTOs
{
    public class PaginacaoRequerimentosDto
    {
        public int PaginaNumero { get; set; } = 1;
        public int PaginaTamanho { get; set; } = 10;
        public string? Status { get; set; }
        public string? Tipo { get; set; }
    }
}
