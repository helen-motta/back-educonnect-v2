namespace Modules.Academico.Application.DTOs
{
    public class CriarAvaliacaoProfessorRequestDto
    {
        public int IdTurma { get; set; }
        public string Nome { get; set; } = string.Empty;
        public DateTime? DataPrevista { get; set; }
        public decimal Peso { get; set; }
    }

    public class LancarFrequenciaProfessorRequestDto
    {
        public int IdMatricula { get; set; }
        public DateTime DataAula { get; set; }
        public bool Presente { get; set; }
        public string? Justificativa { get; set; }
        public int QtdAulas { get; set; }
    }

    public class LancarNotaProfessorRequestDto
    {
        public int IdAvaliacao { get; set; }
        public int IdMatricula { get; set; }
        public decimal ValorObtido { get; set; }
    }
}
