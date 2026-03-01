using Modules.Autenticacao.Domain.Entities;

public class CursoDto
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Descricao { get; set; }
    public int CargaHoraria { get; set; }
    public int CoordernadorId { get; set; }
    public string Codigo { get; set; }
    public Usuario Coordenador { get; set; }
    public List<DisciplinasDto> Disciplinas { get; set; } = new();
}