public class PagedResponse<T>
{
    public PagedResponse(IEnumerable<T> data, int totalRegistros, int pagina, int tamanho)
    {
        Data = data;
        TotalRegistros = totalRegistros;
        PaginaAtual = pagina;
        TotalPaginas = (int)Math.Ceiling(totalRegistros / (double)tamanho);
    }

    public IEnumerable<T> Data { get; set; }
    public int TotalRegistros { get; set; }
    public int PaginaAtual { get; set; }
    public int TotalPaginas { get; set; }
}