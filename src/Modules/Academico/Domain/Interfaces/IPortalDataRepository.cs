using Modules.Academico.Domain.Entities;
using src.Modules.Academico.Domain.Entities;

namespace Modules.Academico.Domain.Interfaces;

public interface IPortalDataRepository
{
    Task<PortalConfiguracao> ObterConfiguracaoAsync();
    Task SalvarConfiguracaoAsync(PortalConfiguracao configuracao);
    Task<List<Turma>> ObterTurmasProfessorAsync(int professorId);
    Task<List<Turma>> ObterTodasTurmasAsync();
    Task<List<Curso>> ObterCursosAsync();
    Task<List<Disciplina>> ObterDisciplinasAsync();
    Task<List<Requerimentos>> ObterRequerimentosPendentesAsync();
    Task<List<Comunicado>> ObterComunicadosAsync(int professorId);
    Task<Comunicado> AdicionarComunicadoAsync(Comunicado comunicado);
    Task<List<Atividade>> ObterAtividadesAsync(int professorId);
    Task<Atividade?> ObterAtividadeAsync(int atividadeId, int professorId);
    Task<Atividade> AdicionarAtividadeAsync(Atividade atividade);
    Task SalvarAtividadeAsync(Atividade atividade);
    Task RemoverAtividadeAsync(Atividade atividade);
    Task<EntregaAtividade?> ObterEntregaAsync(int atividadeId, int alunoId, int professorId);
    Task SalvarEntregaAsync(EntregaAtividade entrega);
    Task<SolicitacaoMatricula> AdicionarSolicitacaoAsync(SolicitacaoMatricula solicitacao);
}
