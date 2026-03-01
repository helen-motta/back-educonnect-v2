using System.ComponentModel.DataAnnotations.Schema;
using Modules.Academico.Domain.ValueObjects;
using Modules.Autenticacao.Domain.Entities;
using src.Modules.Academico.Domain.Entities;

namespace Modules.Academico.Domain.Entities
{
    [Table("turma_slots")]
    public class TurmaSlot // Assume que 'Entity' já tem public int Id { get; set; }
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("id_turma")]
        public int TurmaId { get; set; }

        [Column("codigo_slot")]
        public string CodigoSlot { get; set; } // "M1"

        [Column("dia_semana")]
        public byte DiaSemana { get; set; }
    }
}