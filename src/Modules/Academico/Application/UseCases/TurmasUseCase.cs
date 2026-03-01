using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Modules.Academico.Application.DTOs;
using Modules.Academico.Domain.Entities;
using Modules.Academico.Domain.Interfaces;
using Modules.Autenticacao.Application.DTOs;
using Modules.Autenticacao.Domain.Enums;
using Modules.Autenticacao.Domain.Interfaces;
using src.Modules.Academico.Domain.Entities;

namespace Modules.Academico.Application.UseCases
{
    public class TurmasUseCase
    {
        private readonly ITurmasRepository _turmasRepository;

        public TurmasUseCase(ITurmasRepository turmasRepository)
        {
            _turmasRepository = turmasRepository;
        }

        public async Task<IEnumerable<Turma>> Execute(TurmaListagemDto filtro)
        {
            return await _turmasRepository.Execute(filtro);
        }
    }
}