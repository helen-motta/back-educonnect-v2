using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Modules.Autenticacao.Domain.Entities;

public class DisciplinasDto
{
    public int Id { get; set; }
    public int IdCurso { get; set; }
    public string Nome { get; set; }
    public string Codigo { get; set; }
    public string Ementa { get; set; }
    public int CargaHoraria { get; set; }
    public int Creditos { get; set; }
    public int SemestreIdeal { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCriacao { get; set; }
}