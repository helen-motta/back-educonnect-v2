namespace Modules.Academico.Application.DTOs;

public class RegistrarAuditoriaRequestDto
{
    public string TabelaNome { get; set; } = string.Empty;
    public string EntidadeId { get; set; } = string.Empty;
    public string Operacao { get; set; } = string.Empty;
    public object? DadosAnterior { get; set; }
    public object? DadosAtual { get; set; }
    public string UsuarioId { get; set; } = string.Empty;
    public string? EnderecoIp { get; set; }
    public string? UserAgent { get; set; }
}
