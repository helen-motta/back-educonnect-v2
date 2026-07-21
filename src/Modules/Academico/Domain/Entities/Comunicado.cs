using System.ComponentModel.DataAnnotations.Schema;
using Modules.Autenticacao.Domain.Entities;

namespace Modules.Academico.Domain.Entities;

[Table("comunicados")]
public sealed class Comunicado
{
    [Column("id")]
    public int Id { get; set; }
    [Column("professor_id")]
    public int ProfessorId { get; set; }
    [Column("assunto")]
    public string Assunto { get; set; } = string.Empty;
    [Column("mensagem")]
    public string Mensagem { get; set; } = string.Empty;
    [Column("criado_em")]
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public Usuario Professor { get; set; } = null!;
    public ICollection<ComunicadoTurma> Turmas { get; set; } = new List<ComunicadoTurma>();
}

[Table("comunicados_turmas")]
public sealed class ComunicadoTurma
{
    [Column("comunicado_id")]
    public int ComunicadoId { get; set; }
    [Column("turma_id")]
    public int TurmaId { get; set; }
    public Comunicado Comunicado { get; set; } = null!;
    public Turma Turma { get; set; } = null!;
}
