using System.ComponentModel.DataAnnotations.Schema;

namespace Modules.Academico.Domain.Entities;

[Table("atividades")]
public sealed class Atividade
{
    [Column("id")]
    public int Id { get; set; }
    [Column("turma_id")]
    public int TurmaId { get; set; }
    [Column("titulo")]
    public string Titulo { get; set; } = string.Empty;
    [Column("descricao")]
    public string Descricao { get; set; } = string.Empty;
    [Column("tipo")]
    public string Tipo { get; set; } = "Trabalho";
    [Column("prazo")]
    public DateTime Prazo { get; set; }
    [Column("pontuacao")]
    public decimal Pontuacao { get; set; }
    [Column("status")]
    public string Status { get; set; } = "aberta";
    public Turma Turma { get; set; } = null!;
    public ICollection<EntregaAtividade> Entregas { get; set; } = new List<EntregaAtividade>();
}

[Table("entregas_atividades")]
public sealed class EntregaAtividade
{
    [Column("id")]
    public int Id { get; set; }
    [Column("atividade_id")]
    public int AtividadeId { get; set; }
    [Column("aluno_id")]
    public int AlunoId { get; set; }
    [Column("arquivo_nome")]
    public string ArquivoNome { get; set; } = string.Empty;
    [Column("arquivo_url")]
    public string ArquivoUrl { get; set; } = string.Empty;
    [Column("tipo_arquivo")]
    public string TipoArquivo { get; set; } = string.Empty;
    [Column("enviado_em")]
    public DateTime EnviadoEm { get; set; } = DateTime.UtcNow;
    [Column("nota")]
    public decimal? Nota { get; set; }
    [Column("feedback")]
    public string Feedback { get; set; } = string.Empty;
    public Atividade Atividade { get; set; } = null!;
    public Aluno Aluno { get; set; } = null!;
}
