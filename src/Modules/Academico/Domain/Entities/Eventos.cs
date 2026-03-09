using System.ComponentModel.DataAnnotations.Schema;
using Modules.Autenticacao.Domain.Entities;

public class Eventos
{
    [Column("id")]
    public int Id { get; private set; }
    [Column("titulo")]
    public string Titulo { get; private set; }
    [Column("data_inicio")]
    public DateTime DataInicio { get; private set; }
    [Column("descricao")]
    public string Descricao { get; private set; }
    [Column("tipo")]
    public TipoEvento Tipo { get; private set; }
    [Column("professorId")]
    public int ProfessorId { get; private set; }
    [Column("disciplinaId")]
    public int? DisciplinaId { get; private set; }
    [Column("data_criacao")]
    public DateTime DataCriacao { get; private set; }

    public Eventos(string titulo, DateTime dataInicio, string descricao, TipoEvento tipo, int professorId, int? disciplinaId = null)
    {
        Titulo = titulo;
        DataInicio = dataInicio;
        Descricao = descricao;
        Tipo = tipo;
        ProfessorId = professorId;
        DisciplinaId = disciplinaId;
        DataCriacao = DateTime.Now;
    }

    // EF Core constructor
    private Eventos() { }
}