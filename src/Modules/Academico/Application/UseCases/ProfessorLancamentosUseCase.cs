using Modules.Academico.Application.DTOs;
using Modules.Academico.Domain.Entities;
using Modules.Academico.Domain.Interfaces;

namespace Modules.Academico.Application.UseCases
{
    public class ProfessorLancamentosUseCase
    {
        private readonly IProfessorLancamentosRepository _repository;

        public ProfessorLancamentosUseCase(IProfessorLancamentosRepository repository)
        {
            _repository = repository;
        }

        public async Task<AvaliacaoProfessor> CriarAvaliacaoAsync(int professorId, CriarAvaliacaoProfessorRequestDto request)
        {
            if (request.IdTurma <= 0)
                throw new ArgumentException("IdTurma deve ser maior que zero.");

            if (string.IsNullOrWhiteSpace(request.Nome))
                throw new ArgumentException("Nome da avaliação é obrigatório.");

            if (request.Nome.Length > 50)
                throw new ArgumentException("Nome da avaliação deve ter no máximo 50 caracteres.");

            if (request.Peso <= 0 || request.Peso > 99.99m)
                throw new ArgumentException("Peso deve estar entre 0,01 e 99,99.");

            var professorEhResponsavel = await _repository.ProfessorResponsavelPelaTurmaAsync(professorId, request.IdTurma);
            if (!professorEhResponsavel)
                throw new InvalidOperationException("Professor não está vinculado à turma informada.");

            var avaliacao = new AvaliacaoProfessor
            {
                IdTurma = request.IdTurma,
                Nome = request.Nome.Trim(),
                DataPrevista = request.DataPrevista?.Date,
                Peso = decimal.Round(request.Peso, 2)
            };

            return await _repository.CriarAvaliacaoAsync(avaliacao);
        }

        public async Task<List<AvaliacaoProfessor>> ListarAvaliacoesPorTurmaAsync(int professorId, int turmaId)
        {
            if (turmaId <= 0)
                throw new ArgumentException("TurmaId deve ser maior que zero.");

            var professorEhResponsavel = await _repository.ProfessorResponsavelPelaTurmaAsync(professorId, turmaId);
            if (!professorEhResponsavel)
                throw new InvalidOperationException("Professor não está vinculado à turma informada.");

            return await _repository.ObterAvaliacoesPorTurmaAsync(turmaId);
        }

        public async Task<FrequenciaProfessor> LancarFrequenciaAsync(int professorId, LancarFrequenciaProfessorRequestDto request)
        {
            if (request.IdMatricula <= 0)
                throw new ArgumentException("IdMatricula deve ser maior que zero.");

            if (request.QtdAulas <= 0)
                throw new ArgumentException("QtdAulas deve ser maior que zero.");

            if (!string.IsNullOrWhiteSpace(request.Justificativa) && request.Justificativa.Length > 255)
                throw new ArgumentException("Justificativa deve ter no máximo 255 caracteres.");

            var turmaId = await _repository.ObterTurmaIdPorMatriculaAsync(request.IdMatricula);
            if (turmaId is null)
                throw new InvalidOperationException("Matrícula não encontrada.");

            var professorEhResponsavel = await _repository.ProfessorResponsavelPelaTurmaAsync(professorId, turmaId.Value);
            if (!professorEhResponsavel)
                throw new InvalidOperationException("Professor não está vinculado à turma da matrícula informada.");

            var frequencia = new FrequenciaProfessor
            {
                IdMatricula = request.IdMatricula,
                DataAula = request.DataAula.Date,
                Presente = request.Presente,
                Justificativa = string.IsNullOrWhiteSpace(request.Justificativa) ? null : request.Justificativa.Trim(),
                QtdAulas = request.QtdAulas
            };

            return await _repository.LancarFrequenciaAsync(frequencia);
        }

        public async Task<List<FrequenciaProfessor>> ListarFrequenciasPorMatriculaAsync(int professorId, int matriculaId)
        {
            if (matriculaId <= 0)
                throw new ArgumentException("MatriculaId deve ser maior que zero.");

            var turmaId = await _repository.ObterTurmaIdPorMatriculaAsync(matriculaId);
            if (turmaId is null)
                throw new InvalidOperationException("Matrícula não encontrada.");

            var professorEhResponsavel = await _repository.ProfessorResponsavelPelaTurmaAsync(professorId, turmaId.Value);
            if (!professorEhResponsavel)
                throw new InvalidOperationException("Professor não está vinculado à turma da matrícula informada.");

            return await _repository.ObterFrequenciasPorMatriculaAsync(matriculaId);
        }

        public async Task<NotaProfessor> LancarNotaAsync(int professorId, LancarNotaProfessorRequestDto request)
        {
            if (request.IdAvaliacao <= 0)
                throw new ArgumentException("IdAvaliacao deve ser maior que zero.");

            if (request.IdMatricula <= 0)
                throw new ArgumentException("IdMatricula deve ser maior que zero.");

            if (request.ValorObtido < 0 || request.ValorObtido > 99.99m)
                throw new ArgumentException("ValorObtido deve estar entre 0 e 99,99.");

            var turmaId = await _repository.ObterTurmaIdPorMatriculaAsync(request.IdMatricula);
            if (turmaId is null)
                throw new InvalidOperationException("Matrícula não encontrada.");

            var avaliacaoPertenceATurma = await _repository.AvaliacaoPertenceATurmaAsync(request.IdAvaliacao, turmaId.Value);
            if (!avaliacaoPertenceATurma)
                throw new InvalidOperationException("Avaliação não pertence à turma da matrícula informada.");

            var professorEhResponsavel = await _repository.ProfessorResponsavelPelaTurmaAsync(professorId, turmaId.Value);
            if (!professorEhResponsavel)
                throw new InvalidOperationException("Professor não está vinculado à turma da matrícula informada.");

            var nota = new NotaProfessor
            {
                IdAvaliacao = request.IdAvaliacao,
                IdMatricula = request.IdMatricula,
                ValorObtido = decimal.Round(request.ValorObtido, 2)
            };

            return await _repository.LancarNotaAsync(nota);
        }

        public async Task<List<NotaProfessor>> ListarNotasPorMatriculaAsync(int professorId, int matriculaId)
        {
            if (matriculaId <= 0)
                throw new ArgumentException("MatriculaId deve ser maior que zero.");

            var turmaId = await _repository.ObterTurmaIdPorMatriculaAsync(matriculaId);
            if (turmaId is null)
                throw new InvalidOperationException("Matrícula não encontrada.");

            var professorEhResponsavel = await _repository.ProfessorResponsavelPelaTurmaAsync(professorId, turmaId.Value);
            if (!professorEhResponsavel)
                throw new InvalidOperationException("Professor não está vinculado à turma da matrícula informada.");

            return await _repository.ObterNotasPorMatriculaAsync(matriculaId);
        }
    }
}
