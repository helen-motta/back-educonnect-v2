using Modules.Academico.Domain.Entities;

public class HorarioAlunoDTO
{
    public string CodigoSlot { get; set; }
    public int DiaSemana { get; set; }
    public string Disciplina { get; set; }
    public string Professor { get; set; }
    public string Sala { get; set; }
    public virtual TurmaSlot TurmaSlot { get; set; }
}