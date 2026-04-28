using System.ComponentModel.DataAnnotations.Schema;

namespace Modules.Academico.Domain.Entities
{
    [Table("avaliacoes")]
    public class AvaliacaoProfessor
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("id_turma")]
        public int IdTurma { get; set; }

        [Column("nome")]
        public string Nome { get; set; } = string.Empty;

        [Column("data_prevista")]
        public DateTime? DataPrevista { get; set; }

        [Column("peso")]
        public decimal Peso { get; set; }
    }
}
