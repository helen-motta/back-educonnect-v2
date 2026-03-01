using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modules.Academico.Domain.Entities
{
    [Table("grade_horarios")]
    public class GradeHorario
    {
        [Key]
        [Column("codigo")]
        public string Codigo { get; set; } // "M1", "M2", etc.

        [Column("inicio")]
        public TimeSpan Inicio { get; set; }

        [Column("fim")]
        public TimeSpan Fim { get; set; }

        [Column("descricao")]
        public string Descricao { get; set; } // Opcional: "Matinal 1", "Noturno", etc.

        // Navegação
        public virtual ICollection<TurmaSlot> TurmaSlots { get; set; }
    }
}
