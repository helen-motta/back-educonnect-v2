using Modules.Academico.Domain.Entities;
using Modules.Academico.Domain.Interfaces;

namespace Modules.Academico.Infrastructure.Persistence.Repositories
{
    public class AlunoRepository : IAlunoRepository
    {
        private static readonly List<Aluno> _alunos = new()
        {
            new Aluno(1, "João Silva", "2024001"),
            new Aluno(2, "Maria Santos", "2024002"),
            new Aluno(3, "Pedro Oliveira", "2024003"),
            new Aluno(4, "Ana Costa", "2024004"),
            new Aluno(5, "Carlos Ferreira", "2024005")
        };

        public Task<Aluno?> BuscarPorIdAsync(int alunoId)
        {
            var aluno = _alunos.FirstOrDefault(a => a.Id == alunoId);
            return Task.FromResult(aluno);
        }

        public Task<IEnumerable<Aluno>> BuscarTodosAsync()
        {
            return Task.FromResult(_alunos.AsEnumerable());
        }
    }
}
