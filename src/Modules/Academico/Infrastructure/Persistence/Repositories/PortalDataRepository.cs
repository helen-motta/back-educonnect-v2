using Microsoft.EntityFrameworkCore;
using Modules.Academico.Domain.Entities;
using Modules.Academico.Domain.Interfaces;
using Shared.Infrastructure;
using src.Modules.Academico.Domain.Entities;

namespace Modules.Academico.Infrastructure.Persistence.Repositories;

public sealed class PortalDataRepository : IPortalDataRepository
{
    private readonly AppDbContext _context;
    public PortalDataRepository(AppDbContext context) => _context = context;

    public async Task<PortalConfiguracao> ObterConfiguracaoAsync() =>
        await _context.PortalConfiguracoes.SingleOrDefaultAsync() ?? new PortalConfiguracao();

    public async Task SalvarConfiguracaoAsync(PortalConfiguracao configuracao)
    {
        _context.PortalConfiguracoes.Update(configuracao);
        await _context.SaveChangesAsync();
    }

    public Task<List<Turma>> ObterTurmasProfessorAsync(int professorId) => _context.Turmas.AsNoTracking()
        .Where(x => x.ProfessorId == professorId).Include(x => x.Disciplina).Include(x => x.TurmaSlots)
        .Include(x => x.InscricoesTurmas).ThenInclude(x => x.Aluno).OrderBy(x => x.NomeTurma).ToListAsync();

    public Task<List<Turma>> ObterTodasTurmasAsync() => _context.Turmas.AsNoTracking()
        .Include(x => x.Disciplina).Include(x => x.TurmaSlots).Include(x => x.InscricoesTurmas).OrderBy(x => x.Sala).ToListAsync();

    public Task<List<Curso>> ObterCursosAsync() => _context.Cursos.AsNoTracking().Where(x => x.Ativo).OrderBy(x => x.Nome).ToListAsync();
    public Task<List<Disciplina>> ObterDisciplinasAsync() => _context.Disciplinas.AsNoTracking().ToListAsync();
    public Task<List<Requerimentos>> ObterRequerimentosPendentesAsync() => _context.Requerimentos.AsNoTracking()
        .Where(x => x.Status == "Pendente").Include(x => x.Usuario).OrderBy(x => x.DataAbertura).ToListAsync();

    public Task<List<Comunicado>> ObterComunicadosAsync(int professorId) => _context.Comunicados.AsNoTracking()
        .Where(x => x.ProfessorId == professorId).Include(x => x.Turmas).ThenInclude(x => x.Turma)
        .OrderByDescending(x => x.CriadoEm).ToListAsync();

    public async Task<Comunicado> AdicionarComunicadoAsync(Comunicado comunicado)
    {
        _context.Comunicados.Add(comunicado);
        await _context.SaveChangesAsync();
        return comunicado;
    }

    public Task<List<Atividade>> ObterAtividadesAsync(int professorId) => _context.Atividades.AsNoTracking()
        .Where(x => x.Turma.ProfessorId == professorId).Include(x => x.Entregas).ThenInclude(x => x.Aluno)
        .OrderBy(x => x.Prazo).ToListAsync();

    public Task<Atividade?> ObterAtividadeAsync(int atividadeId, int professorId) => _context.Atividades
        .Include(x => x.Entregas).SingleOrDefaultAsync(x => x.Id == atividadeId && x.Turma.ProfessorId == professorId);

    public async Task<Atividade> AdicionarAtividadeAsync(Atividade atividade)
    {
        _context.Atividades.Add(atividade);
        await _context.SaveChangesAsync();
        return atividade;
    }

    public async Task SalvarAtividadeAsync(Atividade atividade) { await _context.SaveChangesAsync(); }
    public async Task RemoverAtividadeAsync(Atividade atividade) { _context.Atividades.Remove(atividade); await _context.SaveChangesAsync(); }

    public Task<EntregaAtividade?> ObterEntregaAsync(int atividadeId, int alunoId, int professorId) => _context.EntregasAtividades
        .SingleOrDefaultAsync(x => x.AtividadeId == atividadeId && x.AlunoId == alunoId && x.Atividade.Turma.ProfessorId == professorId);

    public async Task SalvarEntregaAsync(EntregaAtividade entrega) { await _context.SaveChangesAsync(); }

    public async Task<SolicitacaoMatricula> AdicionarSolicitacaoAsync(SolicitacaoMatricula solicitacao)
    {
        _context.SolicitacoesMatricula.Add(solicitacao);
        await _context.SaveChangesAsync();
        return solicitacao;
    }
}
