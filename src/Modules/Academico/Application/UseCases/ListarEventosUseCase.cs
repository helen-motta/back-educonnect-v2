using Modules.Academico.Domain.Interfaces;

public class ListarEventosUseCase
{
    private readonly IEventoRepository _eventoRepository;
    private readonly IAlunoRepository _alunoRepository; 

    private readonly ITurmasRepository _turmasRepository;

    public ListarEventosUseCase(IEventoRepository eventoRepository, IAlunoRepository alunoRepository, ITurmasRepository turmasRepository)
    {
        _eventoRepository = eventoRepository;
        _alunoRepository = alunoRepository;
        _turmasRepository = turmasRepository;
    }

    public async Task<IEnumerable<EventoFullCalendarDto>> ExecutarAsync(int usuarioId)
    {
        List<int>? disciplinasDoAluno = null;


        disciplinasDoAluno = await _turmasRepository.GetDisciplinasPorAluno(usuarioId);

        var eventos = await _eventoRepository.ObterEventosVisiveisAsync(disciplinasDoAluno);

        return eventos.Select(e => new EventoFullCalendarDto
        {
            Id = e.Id.ToString(),
            Title = e.Titulo,
            Start = e.DataInicio.ToString("yyyy-MM-ddTHH:mm:ss"),
            ExtendedProps = new ExtendedPropsDto
            {
                Descricao = e.Descricao,
                professorId = e.ProfessorId, 
                Tipo = e.Tipo.ToString(),
            }
        });
    }

    public async Task AdicionarEventoAsync(int professorId, CriarEventoRequestDto request)
    {
            var novoEvento = new Eventos(
                titulo: request.Titulo,
                dataInicio: request.DataInicio,
                descricao: request.Descricao,
                tipo: request.Tipo,
                professorId: professorId, 
                disciplinaId: request.DisciplinaId
            );


            await _eventoRepository.AdicionarAsync(novoEvento);
    }
}