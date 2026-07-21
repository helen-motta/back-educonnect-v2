using System.ComponentModel.DataAnnotations.Schema;
using Modules.Academico.Domain.ValueObjects;
using Modules.Autenticacao.Domain.Entities;
using src.Modules.Academico.Domain.Entities;

namespace Modules.Academico.Domain.Entities
{
    public class Disciplina
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("id_curso")]
        public int IdCurso { get; set; }
        [ForeignKey("IdCurso")]
        public virtual Curso Curso { get; set; } = null!;
        [Column("nome")]
        public string Nome { get; set; } = string.Empty;
        [Column("codigo")]
        public string Codigo { get; set; } = string.Empty;
        [Column("ementa")]
        public string Ementa { get; set; } = string.Empty;
        [Column("carga_horaria")]
        public int CargaHoraria { get; set; }
        [Column("creditos")]
        public int Creditos { get; set; }
        [Column("semestre_ideal")]
        public int SemestreIdeal { get; set; }
        [Column("ativo")]
        public bool Ativo { get; set; }
        [Column("criado_em")]
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    }
}
