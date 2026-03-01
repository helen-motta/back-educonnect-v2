namespace Modules.Academico.Application.DTOs
{
    public class CriarCursoRequest
    {
        public string Nome { get; set; }
        public string Codigo { get; set; }
        public string? Descricao { get; set; }
        public int CargaHoraria { get; set; }
        public int Modalidade { get; set; }
        public int IdCoordenador { get; set; }
    }
}
