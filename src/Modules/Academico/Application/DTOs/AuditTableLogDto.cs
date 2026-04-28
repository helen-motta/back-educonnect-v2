namespace Modules.Academico.Application.DTOs;

public class AuditTableLogDto
{
    public DateTimeOffset DataHora { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Acao { get; set; } = string.Empty;
    public string Detalhes { get; set; } = string.Empty;
    public string Ip { get; set; } = string.Empty;
}
