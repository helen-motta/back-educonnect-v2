using System.ComponentModel.DataAnnotations.Schema;
using Modules.Academico.Domain.Entities;
using Modules.Autenticacao.Domain.Entities;

namespace src.Modules.Academico.Domain.Entities
{
    public class Curso
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("nome")]
        public string Nome { get; set; } = string.Empty;
        [Column("codigo")]
        public string Codigo { get; set; } = string.Empty;
        [Column("descricao")]
        public string? Descricao { get; set; }
        [Column("carga_horaria")]
        public int CargaHoraria { get; set; }
        [Column("modalidade")]
        public int Modalidade { get; set; }
        [Column("ativo")]
        public bool Ativo { get; set; } = true;
        [Column("data_criacao")]
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        [Column("id_coordenador_responsavel")]
        public int IdCoordenador { get; set; }
        [ForeignKey("IdCoordenador")]
        public virtual Usuario Coordenador { get; set; } = null!;
        public virtual List<Disciplina> Disciplinas { get; set; } = new();

    }
}