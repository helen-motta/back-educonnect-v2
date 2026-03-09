public interface IEventoRepository
{
    Task AdicionarAsync(Eventos evento);
    
    Task<IEnumerable<Eventos>> ObterEventosVisiveisAsync(List<int>? disciplinasIds = null);
}