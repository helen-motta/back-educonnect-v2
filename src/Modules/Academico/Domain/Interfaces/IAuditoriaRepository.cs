using Shared.Domain.Entities;

namespace Modules.Academico.Domain.Interfaces;

public interface IAuditoriaRepository
{
    Task AdicionarAsync(Auditoria auditoria);
}
