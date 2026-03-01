namespace Modules.Academico.Application.DTOs
{
    public class DesempenhoAcademicoResponse
    {
        public int AlunoId { get; set; }
        public string NomeAluno { get; set; }
        public int DisciplinaId { get; set; }
        public string NomeDisciplina { get; set; }
        public string PeriodoLetivo { get; set; }
        public decimal MediaFinal { get; set; }
        public decimal FrequenciaPercentual { get; set; }
        public string Situacao { get; set; }
        public string MensagemSituacao { get; set; }
    }
}
