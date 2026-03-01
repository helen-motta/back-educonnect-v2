using Modules.Academico.Domain.ValueObjects;

namespace Modules.Academico.Domain.Entities
{
    public class Frequencia
    {
        public int Id { get; set; }
        public int MatriculaId { get; set; }
        public int AulasAssistidas { get; set; }
        public int TotalAulas { get; set; }
        public bool Consolidada { get; set; }

        public Frequencia(int id, int matriculaId, int aulasAssistidas, int totalAulas)
        {
            if (matriculaId <= 0)
                throw new ArgumentException("MatriculaId deve ser válido");
            if (aulasAssistidas < 0)
                throw new ArgumentException("Aulas assistidas não pode ser negativo");
            if (totalAulas <= 0)
                throw new ArgumentException("Total de aulas deve ser maior que zero");
            if (aulasAssistidas > totalAulas)
                throw new ArgumentException("Aulas assistidas não pode ser maior que total de aulas");

            Id = id;
            MatriculaId = matriculaId;
            AulasAssistidas = aulasAssistidas;
            TotalAulas = totalAulas;
            Consolidada = true;
        }

        public Percentual CalcularPercentual()
        {
            decimal percentual = (decimal)AulasAssistidas / TotalAulas * 100;
            return Percentual.Criar(percentual);
        }
    }
}
