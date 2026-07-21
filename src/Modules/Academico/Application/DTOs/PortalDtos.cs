namespace Modules.Academico.Application.DTOs;

public sealed record PortalConfiguracaoDto(bool FeatureDarkMode, bool FeatureCarteirinha, bool FeatureMatricula, bool FeatureFinanceiro);
public sealed record TurmaResumoDto(int Id, string Nome);
public sealed record ComunicadoDto(int Id, string Assunto, string Mensagem, DateTime CriadoEm, IReadOnlyCollection<TurmaResumoDto> Turmas);
public sealed record CriarComunicadoDto(string Assunto, string Mensagem, IReadOnlyCollection<int> TurmaIds);
public sealed record AlunoResumoDto(int Id, string Nome, string Matricula, string Avatar);
public sealed record EntregaAtividadeDto(int Id, int AlunoId, string Arquivo, string Tipo, string Url, DateTime EnviadoEm, decimal? Nota, string Feedback);
public sealed record AtividadeDto(int Id, int TurmaId, string Titulo, string Descricao, string Tipo, DateTime Prazo, decimal Pontuacao, string Status, IReadOnlyCollection<EntregaAtividadeDto> Entregas);
public sealed record TurmaAtividadesDto(int Id, string Nome, string Codigo, string Semestre, int TotalAlunos, IReadOnlyCollection<AlunoResumoDto> Alunos, IReadOnlyCollection<AtividadeDto> Atividades);
public sealed record SalvarAtividadeDto(int TurmaId, string Titulo, string Descricao, string Tipo, DateTime Prazo, decimal Pontuacao, string Status);
public sealed record AvaliarEntregaDto(decimal? Nota, string? Feedback);
public sealed record SalaReservaDto(int TurmaId, string Turma, string Disciplina, string CodigoSlot, byte DiaSemana);
public sealed record SalaDto(string Nome, IReadOnlyCollection<SalaReservaDto> Reservas);
public sealed record CursoDisponivelDto(int Id, string Nome, string Codigo, string Descricao, int CargaHoraria, int Modalidade);
public sealed record SolicitarMatriculaDto(int CursoId, string NomeCandidato, string Email, string Turno, string Cpf);
public sealed record ProfessorDashboardDto(object? ProximaAula, IReadOnlyCollection<object> AcoesPendentes, IReadOnlyCollection<ComunicadoDto> UltimosComunicados, IReadOnlyCollection<object> ProximasAvaliacoes);
public sealed record CoordenadorDashboardDto(IReadOnlyCollection<object> RequerimentosPendentes, IReadOnlyCollection<object> AlertasAcademicos, object StatsGerais);
