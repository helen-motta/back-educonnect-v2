using System.Threading.Tasks;
using Modules.Academico.Application.DTOs;
using Modules.Academico.Domain.Interfaces;

namespace Modules.Academico.Application.UseCases
{
    public class RequerimentosUseCase
    {
        private readonly IRequerimentosRepository _requerimentosRepository;

        public RequerimentosUseCase(IRequerimentosRepository requerimentosRepository)
        {
            _requerimentosRepository = requerimentosRepository;
        }

        public async Task<Requerimentos> CriarRequerimentoAsync(int idUsuario, string tipo, string? observacao)
        {
            var requerimento = new Requerimentos
            {
                IdUsuario = idUsuario,
                TipoSolicitacao = tipo,
                Observacao = observacao,
                Status = "Aberto",
                DataAbertura = DateTime.UtcNow
            };

            return await _requerimentosRepository.AdicionarAsync(requerimento);
        }

        public async Task<List<Requerimentos>> ObterRequerimentosPorUsuarioAsync(int idUsuario)
        {
            return await _requerimentosRepository.BuscarPorUsuarioAsync(idUsuario);
        }

        public async Task<Requerimentos?> ObterRequerimentoPorIdAsync(int id)
        {
            return await _requerimentosRepository.BuscarPorIdAsync(id);
        }

        public async Task AtualizarStatusRequerimentoAsync(int id, string novoStatus, string? respostaAdmin)
        {
            var requerimento = await _requerimentosRepository.BuscarPorIdAsync(id);
            if (requerimento != null)
            {
                requerimento.Status = novoStatus;
                requerimento.RespostaAdmin = respostaAdmin;
                if (novoStatus == "Concluído" || novoStatus == "Recusado")
                {
                    requerimento.DataConclusao = DateTime.UtcNow;
                }
                await _requerimentosRepository.AtualizarAsync(requerimento);
            }
        }

        public async Task<PagedResponse<Requerimentos>> ListarRequerimentosPaginadosAsync(PaginacaoRequerimentosDto filtro)
        {
            var (requerimentos, total) = await _requerimentosRepository.ListarRequerimentosPaginados(filtro);
            return new PagedResponse<Requerimentos>(requerimentos, total, filtro.PaginaNumero, filtro.PaginaTamanho);
        }
    }
}
