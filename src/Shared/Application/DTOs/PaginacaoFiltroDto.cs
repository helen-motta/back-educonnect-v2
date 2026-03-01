public class PaginacaoFiltroDto
{
    public int PaginaNumero { get; set; } = 1;
    public int PaginaTamanho { get; set; } = 10;
    public string? Nome { get; set; }
    public string? Email { get; set; }
    public int? IdPerfil { get; set; }
    public string? Status { get; set; }
    public string? Registro { get; set; }
}