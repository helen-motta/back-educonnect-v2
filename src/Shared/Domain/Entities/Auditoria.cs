namespace Shared.Domain.Entities;

public class Auditoria
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TabelaNome { get; set; } = string.Empty;
    public string EntidadeId { get; set; } = string.Empty;
    public string Operacao { get; set; } = string.Empty;
    public string? DadosAnterior { get; set; }
    public string? DadosAtual { get; set; }
    public string UsuarioId { get; set; } = string.Empty;
    public DateTimeOffset DataHora { get; set; } = DateTimeOffset.UtcNow;
    public string? EnderecoIp { get; set; }
    public string? UserAgent { get; set; }
}
