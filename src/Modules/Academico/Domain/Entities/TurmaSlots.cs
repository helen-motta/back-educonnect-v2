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
        public string CodigoSlot { get; set; } = string.Empty; // "M1"

        public virtual Turma Turma { get; set; } = null!;

        [Column("dia_semana")]
        public byte DiaSemana { get; set; }

        public virtual string DiaSemanaNome => DiaSemana switch
        {
            2 => "Segunda-feira",
            3 => "Terça-feira",
            4 => "Quarta-feira",
            5 => "Quinta-feira",
            6 => "Sexta-feira",
            _ => "Desconhecido"
        };

        public virtual string Horario => CodigoSlot switch
        {
            "M1" => "07:00 - 08:40",
            "M2" => "08:40 - 10:20",
            "M3" => "10:20 - 12:00",
            "T1" => "13:00 - 14:40",
            "T2" => "14:40 - 16:20",
            "T3" => "16:20 - 18:00",
            "N1" => "19:00 - 20:40",
            "N2" => "20:40 - 22:20",
            _ => "Desconhecido"
        };  
    }
}
