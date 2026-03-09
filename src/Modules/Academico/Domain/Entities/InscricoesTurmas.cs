using System.ComponentModel.DataAnnotations.Schema;
using Modules.Academico.Domain.ValueObjects;
using Modules.Autenticacao.Domain.Entities;
using src.Modules.Academico.Domain.Entities;

namespace Modules.Academico.Domain.Entities
{
    [Table("inscricoes_turmas")]
    public class InscricoesTurmas 
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("aluno_id")]
        public int AlunoId { get; set; }

        [Column("turma_id")]
        public int TurmaId { get; set; }

        [Column("p1")]
        public decimal? P1 { get; set; } = null;

        [Column("p2")]
        public decimal? P2 { get; set; } = null;

        [Column("recuperacao")]
        public decimal? Recuperacao { get; set; } = null;

        [Column("trabalho")]
        public decimal? Trabalho { get; set; } = null;

        [Column("nota_final")]
        public decimal? NotaFinal { get; set; } = null;

        [Column("frequencia")]
        public int? Frequencia { get; set; } = 0;

        [Column("status")]
        public string Status { get; set; } = "Ativo";

        [Column("papel")]
        public string Papel { get; set; } = "Aluno";

        [Column("data_inscricao")]
        public DateTime DataInscricao { get; set; } = DateTime.UtcNow;

        public virtual Turma Turma { get; set; } 
        public virtual Aluno Aluno { get; set; }
    }
}