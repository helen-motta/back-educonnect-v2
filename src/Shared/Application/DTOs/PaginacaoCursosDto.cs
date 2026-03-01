using Modules.Autenticacao.Domain.Entities;

public class PaginacaoCursosDto
{
    public int PaginaNumero { get; set; } = 1;
    public int PaginaTamanho { get; set; } = 10;
    public string? Nome { get; set; }
    public string? Codigo { get; set; }
    public string? Descricao { get; set; }
    public int CargaHoraria { get; set; }
    public string? Modalidade { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime DataCriacao { get; set; }
    public Usuario? Coordenador { get; set; }    
}