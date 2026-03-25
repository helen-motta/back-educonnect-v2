using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Modules.Academico.Application.DTOs;
using Modules.Academico.Domain.Interfaces;
using Modules.Autenticacao.Application.DTOs;
using Modules.Autenticacao.Domain.Enums;
using Modules.Autenticacao.Domain.Interfaces;
using src.Modules.Academico.Domain.Entities;

namespace Modules.Autenticacao.Application.UseCases
{
    public class CursosUseCase
    {
        private readonly ICursosRepository _cursosRepository;

        public CursosUseCase(ICursosRepository cursosRepository)
        {
            _cursosRepository = cursosRepository;
        }

        public async Task<Curso?> ObterPorIdAsync(int id)
        {
            return await _cursosRepository.BuscarPorIdAsync(id);
        }

        public async Task<Curso> CriarCursoAsync(CriarCursoRequest request)
        {
            var curso = new Curso
            {
                Nome = request.Nome,
                Codigo = request.Codigo,
                Descricao = request.Descricao,
                CargaHoraria = request.CargaHoraria,
                Modalidade = request.Modalidade,
                IdCoordenador = request.IdCoordenador,
                Ativo = true,
                DataCriacao = DateTime.UtcNow
            };

            return await _cursosRepository.AdicionarAsync(curso);
        }

        public async Task<Curso?> AtualizarCursoAsync(int id, CriarCursoRequest request)
        {
            var curso = await _cursosRepository.BuscarPorIdAsync(id);
            if (curso == null)
            {
                return null;
            }

            curso.Nome = request.Nome;
            curso.Codigo = request.Codigo;
            curso.Descricao = request.Descricao;
            curso.CargaHoraria = request.CargaHoraria;
            curso.Modalidade = request.Modalidade;
            curso.IdCoordenador = request.IdCoordenador;

            await _cursosRepository.AtualizarAsync(curso);
            return curso;
        }

        public async Task<PagedResponse<CursoDto>> Execute(PaginacaoCursosDto filtro)
        {
            var (cursos, total) = await _cursosRepository.ListarCursosPaginados(filtro);


            var listaDto = cursos.Select(u => new CursoDto
            {
                Id = u.Id,
                Nome = u.Nome,
                Codigo = u.Codigo,
                Descricao = u.Descricao,
                Coordenador = u.Coordenador,
                CargaHoraria = u.CargaHoraria,
                Disciplinas = u.Disciplinas.Select(d => new DisciplinasDto
                {
                    Id = d.Id,
                    Nome = d.Nome,
                    Codigo = d.Codigo,
                    SemestreIdeal = d.SemestreIdeal,
                    CargaHoraria = d.CargaHoraria,
                    Ementa = d.Ementa
                })
                .OrderBy(d => d.SemestreIdeal)
                .ToList()
            }).ToList();

            return new PagedResponse<CursoDto>(listaDto, total, filtro.PaginaNumero, filtro.PaginaTamanho);
        }
    }
}