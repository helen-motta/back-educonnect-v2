namespace Modules.Academico.Application.DTOs;

public class PaginacaoAuditLogsDto
{
    public int PaginaNumero { get; set; } = 1;
    public int PaginaTamanho { get; set; } = 10;
    public string? Usuario { get; set; }
    public string? Tipo { get; set; }
    public string? Acao { get; set; }
    public DateTimeOffset? DataInicio { get; set; }
    public DateTimeOffset? DataFim { get; set; }
}
