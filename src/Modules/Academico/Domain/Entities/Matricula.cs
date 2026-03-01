using System.ComponentModel.DataAnnotations.Schema;
using Modules.Autenticacao.Domain.Entities;

namespace Modules.Academico.Domain.Entities
{
    [Table("matriculas")]
    public class Matricula
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("id_turma")]
        public int TurmaId { get; set; }

        [Column("id_aluno")]
        public int AlunoId { get; set; }

        [Column("id_disciplina")]
        public int DisciplinaId { get; set; }

        [Column("periodo_letivo")]
        public string PeriodoLetivo { get; set; } = string.Empty;

        [Column("data_matricula")]
        public DateTime DataMatricula { get; set; } = DateTime.Now;

        // Relacionamentos (Navigation Properties)
        [ForeignKey("TurmaId")]
        public virtual Turma? Turma { get; set; }

        [ForeignKey("AlunoId")]
        public virtual Usuario? Aluno { get; set; }

        public Matricula() { }

        public Matricula(int id, int alunoId, int disciplinaId, string periodoLetivo)
        {
            Id = id;
            AlunoId = alunoId;
            DisciplinaId = disciplinaId;
            PeriodoLetivo = periodoLetivo;
            DataMatricula = DateTime.Now;
        }
    }
}