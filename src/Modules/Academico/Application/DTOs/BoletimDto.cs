using Modules.Academico.Application.DTOs;

namespace SeuProjeto.Modules.Academico.Application.DTOs
{
    public class BoletimDto
    {
        public string NomeAluno { get; set; }
        public string PeriodoLetivo { get; set; }
        // Aqui reutilizamos a classe de resposta que você já criou
        public List<DesempenhoAcademicoResponse> Desempenhos { get; set; } = new();
    }
}