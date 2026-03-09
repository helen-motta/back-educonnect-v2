using Modules.Autenticacao.Domain.Entities;

public class EventoFullCalendarDto
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Start { get; set; } // Formato ISO "YYYY-MM-DDTHH:mm:ss"
    public ExtendedPropsDto ExtendedProps { get; set; }
}

public class ExtendedPropsDto
{
    public string Descricao { get; set; }
    public int professorId { get; set; }
    public string Tipo { get; set; } 
}