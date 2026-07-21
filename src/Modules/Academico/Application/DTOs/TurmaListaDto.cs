namespace Modules.Academico.Application.DTOs;

public sealed class TurmaListaDTO
{
    public int Id { get; set; }
    public string NomeTurma { get; set; } = string.Empty;
    public int DisciplinaId { get; set; }
    public string DisciplinaNome { get; set; } = string.Empty;
    public int ProfessorId { get; set; }
    public string ProfessorNome { get; set; } = string.Empty;
    public string Sala { get; set; } = string.Empty;
    public int Vagas { get; set; }
    public int QuantidadeInscritos { get; set; }
    public List<string> HorariosFormatados { get; set; } = [];
}

public sealed record AlunoTurmaDto(int Id, int MatriculaId, string Nome, string Ra, decimal? P1, decimal? P2, decimal? Trabalho);
public sealed record AvaliacaoTurmaDto(int Id, string Nome, decimal Peso, DateTime? DataPrevista);
public sealed record TurmaDetalheDto(int Id, string NomeTurma, string Sala, string DisciplinaNome, IReadOnlyCollection<AlunoTurmaDto> Alunos, IReadOnlyCollection<AvaliacaoTurmaDto> Atividades);
public sealed record SalvarTurmaDto(string NomeTurma, int? DisciplinaId, int? ProfessorId, string? Sala, int? Vagas);
