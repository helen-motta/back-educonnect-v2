using System.ComponentModel.DataAnnotations.Schema;

namespace Modules.Academico.Domain.Entities
{
    [Table("frequencias")]
    public class FrequenciaProfessor
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("id_matricula")]
        public int IdMatricula { get; set; }

        [Column("data_aula")]
        public DateTime DataAula { get; set; }

        [Column("presente")]
        public bool Presente { get; set; }

        [Column("justificativa")]
        public string? Justificativa { get; set; }

        [Column("qtd_aulas")]
        public int QtdAulas { get; set; }
    }
}
