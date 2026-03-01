using System.Text.Json.Serialization;

namespace Modules.Academico.Application.DTOs
{
    public class CriarDisciplinaRequest
    {
        [JsonPropertyName("id_curso")]
        public int IdCurso { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("codigo")]
        public string Codigo { get; set; }

        [JsonPropertyName("ementa")]
        public string? Ementa { get; set; }

        [JsonPropertyName("carga_horaria")]
        public int CargaHoraria { get; set; }

        [JsonPropertyName("creditos")]
        public int? Creditos { get; set; }

        [JsonPropertyName("semestre")]
        public int SemestreIdeal { get; set; }
    }
}
