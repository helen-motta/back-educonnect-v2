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

        public async Task<IEnumerable<TurmaListaDTO>> GetTurmasById(int professorId)
        {
            return await _turmasRepository.GetTurmaById(professorId);
        }

        public async Task<IEnumerable<HorarioAlunoDTO>> GetHorariosPorAluno(int alunoId)
        {
            return await _turmasRepository.GetHorariosPorAluno(alunoId);
        }

        public async Task<IEnumerable<NotasFrequenciaDto>> GetNotasEFrequenciaPorAluno(int alunoId)
        {
            return await _turmasRepository.GetNotasEFrequenciaPorAluno(alunoId);
        }

        public async Task<List<TurmaListaDTO>> GetTurmasPorProfessor(int professorId)
        {
            return await _turmasRepository.GetTurmasPorProfessor(professorId);
        }
    }
}