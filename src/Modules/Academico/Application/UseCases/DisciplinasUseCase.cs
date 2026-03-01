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
    public class DisciplinasUseCase
    {
        private readonly IDisciplinasRepository _disciplinasRepository;

        public DisciplinasUseCase(IDisciplinasRepository disciplinasRepository)
        {
            _disciplinasRepository = disciplinasRepository;
        }

        public async Task<List<Disciplina>?> ObterPorIdCursoAsync(int idCurso)
        {
            return await _disciplinasRepository.BuscarPorCursoId(idCurso);
        }

        public async Task<Disciplina> CriarDisciplinaAsync(CriarDisciplinaRequest request)
        {
            var disciplina = new Disciplina
            {
                IdCurso = request.IdCurso,
                Nome = request.Nome,
                Codigo = request.Codigo,
                Ementa = request.Ementa,
                CargaHoraria = request.CargaHoraria,
                Creditos = request.Creditos ?? 0,
                SemestreIdeal = request.SemestreIdeal,
                Ativo = true,
                DataCriacao = DateTime.UtcNow
            };

            return await _disciplinasRepository.AdicionarAsync(disciplina);
        }
    }
}