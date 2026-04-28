using System.ComponentModel.DataAnnotations.Schema;

namespace Modules.Academico.Domain.Entities
{
    [Table("notas")]
    public class NotaProfessor
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("id_avaliacao")]
        public int IdAvaliacao { get; set; }

        [Column("id_matricula")]
        public int IdMatricula { get; set; }

        [Column("valor_obtido")]
        public decimal ValorObtido { get; set; }
    }
}
