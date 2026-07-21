using Modules.Academico.Application.DTOs;
using Modules.Academico.Domain.Entities;
using Modules.Academico.Domain.Interfaces;

namespace Modules.Academico.Application.UseCases;

public sealed class PortalUseCase
{
    private readonly IPortalDataRepository _repository;
    public PortalUseCase(IPortalDataRepository repository) => _repository = repository;

    public async Task<PortalConfiguracaoDto> ObterConfiguracaoAsync()
    {
        var x = await _repository.ObterConfiguracaoAsync();
        return new(x.FeatureDarkMode, x.FeatureCarteirinha, x.FeatureMatricula, x.FeatureFinanceiro);
    }

    public async Task<PortalConfiguracaoDto> SalvarConfiguracaoAsync(PortalConfiguracaoDto dto)
    {
        var x = await _repository.ObterConfiguracaoAsync();
        x.FeatureDarkMode = dto.FeatureDarkMode; x.FeatureCarteirinha = dto.FeatureCarteirinha;
        x.FeatureMatricula = dto.FeatureMatricula; x.FeatureFinanceiro = dto.FeatureFinanceiro;
        await _repository.SalvarConfiguracaoAsync(x);
        return dto;
    }

    public async Task<IReadOnlyCollection<ComunicadoDto>> ObterComunicadosAsync(int professorId) =>
        (await _repository.ObterComunicadosAsync(professorId)).Select(MapNotice).ToList();

    public async Task<ComunicadoDto> CriarComunicadoAsync(int professorId, CriarComunicadoDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Assunto) || string.IsNullOrWhiteSpace(dto.Mensagem) || dto.TurmaIds.Count == 0)
            throw new ArgumentException("Assunto, mensagem e ao menos uma turma são obrigatórios.");
        var allowedIds = (await _repository.ObterTurmasProfessorAsync(professorId)).Select(x => x.Id).ToHashSet();
        if (dto.TurmaIds.Any(x => !allowedIds.Contains(x))) throw new UnauthorizedAccessException("Turma inválida para este professor.");
        var notice = new Comunicado { ProfessorId = professorId, Assunto = dto.Assunto.Trim(), Mensagem = dto.Mensagem.Trim() };
        foreach (var id in dto.TurmaIds.Distinct()) notice.Turmas.Add(new ComunicadoTurma { TurmaId = id });
        await _repository.AdicionarComunicadoAsync(notice);
        return (await ObterComunicadosAsync(professorId)).First(x => x.Id == notice.Id);
    }

    public async Task<IReadOnlyCollection<TurmaAtividadesDto>> ObterAtividadesAsync(int professorId)
    {
        var classes = await _repository.ObterTurmasProfessorAsync(professorId);
        var activities = (await _repository.ObterAtividadesAsync(professorId)).GroupBy(x => x.TurmaId).ToDictionary(x => x.Key);
        return classes.Select(x => new TurmaAtividadesDto(
            x.Id, x.Disciplina.Nome, x.NomeTurma, $"{DateTime.UtcNow.Year}.2", x.InscricoesTurmas.Count,
            x.InscricoesTurmas.Select(i => new AlunoResumoDto(i.AlunoId, i.Aluno.Nome, i.Aluno.Matricula, Initials(i.Aluno.Nome))).ToList(),
            activities.GetValueOrDefault(x.Id)?.Select(MapActivity).ToList() ?? [])).ToList();
    }

    public async Task<AtividadeDto> CriarAtividadeAsync(int professorId, SalvarAtividadeDto dto)
    {
        ValidateActivity(dto);
        var classes = await _repository.ObterTurmasProfessorAsync(professorId);
        if (classes.All(x => x.Id != dto.TurmaId)) throw new UnauthorizedAccessException("Turma inválida para este professor.");
        var item = new Atividade { TurmaId = dto.TurmaId, Titulo = dto.Titulo.Trim(), Descricao = dto.Descricao.Trim(), Tipo = dto.Tipo, Prazo = dto.Prazo, Pontuacao = dto.Pontuacao, Status = dto.Status };
        return MapActivity(await _repository.AdicionarAtividadeAsync(item));
    }

    public async Task<AtividadeDto> AtualizarAtividadeAsync(int professorId, int id, SalvarAtividadeDto dto)
    {
        ValidateActivity(dto);
        var item = await _repository.ObterAtividadeAsync(id, professorId) ?? throw new KeyNotFoundException("Atividade não encontrada.");
        item.Titulo = dto.Titulo.Trim(); item.Descricao = dto.Descricao.Trim(); item.Tipo = dto.Tipo;
        item.Prazo = dto.Prazo; item.Pontuacao = dto.Pontuacao; item.Status = dto.Status;
        await _repository.SalvarAtividadeAsync(item);
        return MapActivity(item);
    }

    public async Task RemoverAtividadeAsync(int professorId, int id)
    {
        var item = await _repository.ObterAtividadeAsync(id, professorId) ?? throw new KeyNotFoundException("Atividade não encontrada.");
        await _repository.RemoverAtividadeAsync(item);
    }

    public async Task AvaliarEntregaAsync(int professorId, int atividadeId, int alunoId, AvaliarEntregaDto dto)
    {
        var submission = await _repository.ObterEntregaAsync(atividadeId, alunoId, professorId) ?? throw new KeyNotFoundException("Entrega não encontrada.");
        if (dto.Nota is < 0 or > 100) throw new ArgumentException("Nota fora do intervalo permitido.");
        submission.Nota = dto.Nota; submission.Feedback = dto.Feedback?.Trim() ?? string.Empty;
        await _repository.SalvarEntregaAsync(submission);
    }

    public async Task<IReadOnlyCollection<SalaDto>> ObterSalasAsync() => (await _repository.ObterTodasTurmasAsync())
        .GroupBy(x => x.Sala).Select(group => new SalaDto(group.Key, group.SelectMany(x => x.TurmaSlots.Select(slot =>
            new SalaReservaDto(x.Id, x.NomeTurma, x.Disciplina.Nome, slot.CodigoSlot, slot.DiaSemana))).ToList())).ToList();

    public async Task<IReadOnlyCollection<CursoDisponivelDto>> ObterCursosDisponiveisAsync() => (await _repository.ObterCursosAsync())
        .Select(x => new CursoDisponivelDto(x.Id, x.Nome, x.Codigo, x.Descricao ?? string.Empty, x.CargaHoraria, x.Modalidade)).ToList();

    public async Task<int> SolicitarMatriculaAsync(SolicitarMatriculaDto dto, int? usuarioId)
    {
        if (dto.CursoId <= 0 || string.IsNullOrWhiteSpace(dto.NomeCandidato) || string.IsNullOrWhiteSpace(dto.Email))
            throw new ArgumentException("Curso, nome e e-mail são obrigatórios.");
        var item = await _repository.AdicionarSolicitacaoAsync(new SolicitacaoMatricula
        {
            UsuarioId = usuarioId, CursoId = dto.CursoId, NomeCandidato = dto.NomeCandidato.Trim(), Email = dto.Email.Trim(), Turno = dto.Turno.Trim(), Cpf = dto.Cpf.Trim()
        });
        return item.Id;
    }

    public async Task<ProfessorDashboardDto> ObterDashboardProfessorAsync(int professorId)
    {
        var classes = await _repository.ObterTurmasProfessorAsync(professorId);
        var activities = await _repository.ObterAtividadesAsync(professorId);
        var notices = (await _repository.ObterComunicadosAsync(professorId)).Take(3).Select(MapNotice).ToList();
        var nextClass = classes.SelectMany(x => x.TurmaSlots.Select(slot => new { disciplina = $"{x.Disciplina.Nome} - {x.NomeTurma}", horario = slot.Horario, sala = x.Sala, totalAlunos = x.InscricoesTurmas.Count, turmaId = x.Id })).FirstOrDefault();
        var pending = activities.Where(x => x.Status == "aberta").Take(5).Select(x => (object)new { id = x.Id, msg = $"Acompanhar entregas: {x.Titulo}", link = "/dashboard/atividades-turma" }).ToList();
        var evaluations = activities.Where(x => x.Prazo >= DateTime.UtcNow).Take(5).Select(x => (object)new { id = x.Id, tipo = x.Titulo, turma = classes.First(t => t.Id == x.TurmaId).NomeTurma, data = x.Prazo }).ToList();
        return new(nextClass, pending, notices, evaluations);
    }

    public async Task<CoordenadorDashboardDto> ObterDashboardCoordenadorAsync()
    {
        var requests = (await _repository.ObterRequerimentosPendentesAsync()).Select(x => (object)new { id = x.Id, tipo = x.TipoSolicitacao, aluno = x.Usuario.Nome }).ToList();
        var courses = await _repository.ObterCursosAsync(); var subjects = await _repository.ObterDisciplinasAsync(); var classes = await _repository.ObterTodasTurmasAsync();
        var alerts = subjects.Where(x => string.IsNullOrWhiteSpace(x.Ementa)).Select(x => (object)new { id = x.Id, msg = $"Disciplina '{x.Nome}' está sem ementa.", link = "/dashboard/gerenciar-cursos" }).ToList();
        return new(requests, alerts, new { totalCursos = courses.Count, totalDisciplinas = subjects.Count, totalTurmasAbertas = classes.Count });
    }

    private static ComunicadoDto MapNotice(Comunicado x) => new(x.Id, x.Assunto, x.Mensagem, x.CriadoEm, x.Turmas.Select(t => new TurmaResumoDto(t.TurmaId, t.Turma.NomeTurma)).ToList());
    private static AtividadeDto MapActivity(Atividade x) => new(x.Id, x.TurmaId, x.Titulo, x.Descricao, x.Tipo, x.Prazo, x.Pontuacao, x.Status,
        x.Entregas.Select(e => new EntregaAtividadeDto(e.Id, e.AlunoId, e.ArquivoNome, e.TipoArquivo, e.ArquivoUrl, e.EnviadoEm, e.Nota, e.Feedback)).ToList());
    private static string Initials(string name) => string.Concat(name.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2).Select(x => char.ToUpperInvariant(x[0])));
    private static void ValidateActivity(SalvarAtividadeDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Titulo) || string.IsNullOrWhiteSpace(dto.Descricao) || dto.Pontuacao <= 0)
            throw new ArgumentException("Título, descrição e pontuação são obrigatórios.");
    }
}
