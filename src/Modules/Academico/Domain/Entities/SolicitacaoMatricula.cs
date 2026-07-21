using System.ComponentModel.DataAnnotations.Schema;

namespace Modules.Academico.Domain.Entities;

[Table("solicitacoes_matricula")]
public sealed class SolicitacaoMatricula
{
    [Column("id")]
    public int Id { get; set; }
    [Column("usuario_id")]
    public int? UsuarioId { get; set; }
    [Column("curso_id")]
    public int CursoId { get; set; }
    [Column("nome_candidato")]
    public string NomeCandidato { get; set; } = string.Empty;
    [Column("email")]
    public string Email { get; set; } = string.Empty;
    [Column("turno")]
    public string Turno { get; set; } = string.Empty;
    [Column("cpf")]
    public string Cpf { get; set; } = string.Empty;
    [Column("status")]
    public string Status { get; set; } = "Pendente";
    [Column("criado_em")]
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
