using System.ComponentModel.DataAnnotations;

public class GradeHorario
{
    [Key]
    public string Codigo { get; set; }
    public TimeSpan Inicio { get; set; }
    public TimeSpan Fim { get; set; }
}