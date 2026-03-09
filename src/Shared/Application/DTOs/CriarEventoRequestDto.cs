
using Modules.Autenticacao.Domain.Entities;

public class CriarEventoRequestDto
    {
        public string Titulo { get; set; }

        public DateTime DataInicio { get; set; }

        public string Descricao { get; set; }

        public TipoEvento Tipo { get; set; } // 1: Seminário, 2: Workshop, 3: Disciplina

        public int? DisciplinaId { get; set; }

        public Usuario Professor { get; set; }
    }
