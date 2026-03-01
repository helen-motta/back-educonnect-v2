using Modules.Autenticacao.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

public class Requerimentos
{
    [Column("id")]
    public int Id { get; set; }
    [Column("id_usuario")]
    public int IdUsuario { get; set; }
    [Column("tipo_solicitacao")]
    public string TipoSolicitacao { get; set; } 
    [Column("status")]
    public string Status { get; set; }
    [Column("observacao")]
    public string? Observacao { get; set; }
    [Column("resposta_admin")]
    public string? RespostaAdmin { get; set; }
    [Column("data_abertura")]
    public DateTime DataAbertura { get; set; } = DateTime.UtcNow;
    [Column("data_conclusao")]
    public DateTime? DataConclusao { get; set; }
    [ForeignKey("IdUsuario")]
    public virtual Usuario Usuario { get; set; }
}