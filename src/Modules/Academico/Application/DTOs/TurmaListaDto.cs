public class TurmaListaDTO
{
    public int Id { get; set; }
    public string NomeTurma { get; set; }
    public string DisciplinaNome { get; set; }
    public int Vagas { get; set; }
    public int QuantidadeInscritos { get; set; }
    public List<string> HorariosFormatados { get; set; } = new List<string>();
}