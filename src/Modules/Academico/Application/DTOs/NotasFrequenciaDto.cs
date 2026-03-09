namespace Modules.Academico.Application.DTOs
{
    public class NotasFrequenciaDto
    {
        public int TurmaId { get; set; }
        public string Disciplina { get; set; } = string.Empty;
        public string Professor { get; set; } = string.Empty;
        public decimal P1 { get; set; }
        public decimal P2 { get; set; }
        public decimal Trabalho { get; set; }
        public decimal? NotaFinal { get; set; }

        /// <summary>
        /// Frequência percentual do aluno na turma (0-100).
        /// </summary>
        public int FrequenciaPercentual { get; set; }
    }
}