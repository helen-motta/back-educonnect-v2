using System.ComponentModel.DataAnnotations.Schema;
using Modules.Academico.Domain.ValueObjects;
using Modules.Autenticacao.Domain.Entities;
using src.Modules.Academico.Domain.Entities;

namespace Modules.Academico.Domain.Entities
{
    public class Turma
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("nome_turma")]
        public string NomeTurma { get; set; }
        [Column("sala")]
        public string Sala { get; set; }
        [Column("vagas")]
        public int Vagas { get; set; }

        [Column("id_disciplina")]
        public int DisciplinaId { get; set; }
        [Column("id_professor")]
        public int ProfessorId { get; set; }

        public virtual Disciplina Disciplina { get; set; }
        public virtual Usuario Professor { get; set; }

        public virtual TurmaSlot TurmaSlots { get; set; }
    }
}