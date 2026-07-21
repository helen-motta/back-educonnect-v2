using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modules.Academico.Application.DTOs;
using Modules.Autenticacao.Domain.Enums;
using Shared.Infrastructure;

[ApiController]
[Route("api/[controller]")]
public class AuditController : ControllerBase
{
	private readonly AppDbContext _context;

	public AuditController(AppDbContext context)
	{
		_context = context;
	}

	[HttpGet("dashboard")]
	public async Task<IActionResult> GetDashboard([FromQuery] int logsLimit = 10)
	{
		var safeLimit = NormalizarLimiteLogs(logsLimit);
		var stats = await MontarStatsAsync();
		var recentLogs = await BuscarAtividadesRecentesAsync(safeLimit);

		return Ok(new AdminDashboardDto
		{
			Stats = stats,
			RecentLogs = recentLogs
		});
	}

	[HttpGet("dashboard/stats")]
	public async Task<IActionResult> GetDashboardStats()
	{
		var stats = await MontarStatsAsync();
		return Ok(stats);
	}

	[HttpGet("dashboard/recent-logs")]
	public async Task<IActionResult> GetRecentLogs([FromQuery] int limit = 10)
	{
		var safeLimit = NormalizarLimiteLogs(limit);
		var recentLogs = await BuscarAtividadesRecentesAsync(safeLimit);
		return Ok(recentLogs);
	}

	[HttpGet("logs")]
	public async Task<IActionResult> GetLogsTable([FromQuery] PaginacaoAuditLogsDto filtro)
	{
		var pagina = NormalizarPagina(filtro.PaginaNumero);
		var tamanho = NormalizarTamanhoPagina(filtro.PaginaTamanho);

		var query = _context.Auditorias.AsNoTracking().AsQueryable();

		if (!string.IsNullOrWhiteSpace(filtro.Usuario))
			query = query.Where(a => a.UsuarioId.Contains(filtro.Usuario));

		if (!string.IsNullOrWhiteSpace(filtro.Tipo))
			query = query.Where(a => a.TabelaNome.Contains(filtro.Tipo));

		if (!string.IsNullOrWhiteSpace(filtro.Acao))
			query = query.Where(a => a.Operacao.Contains(filtro.Acao));

		var registros = await query.ToListAsync();
		var filtrados = registros.AsEnumerable();

		if (filtro.DataInicio.HasValue)
			filtrados = filtrados.Where(a => a.DataHora >= filtro.DataInicio.Value);

		if (filtro.DataFim.HasValue)
			filtrados = filtrados.Where(a => a.DataHora <= filtro.DataFim.Value);

		var totalRegistros = filtrados.Count();

		var logs = filtrados
			.OrderByDescending(a => a.DataHora)
			.Skip((pagina - 1) * tamanho)
			.Take(tamanho)
			.Select(a => new AuditTableLogDto
			{
				DataHora = a.DataHora,
				Usuario = a.UsuarioId,
				Tipo = a.TabelaNome,
				Acao = a.Operacao,
				Detalhes = !string.IsNullOrWhiteSpace(a.DadosAtual)
					? a.DadosAtual!
					: (!string.IsNullOrWhiteSpace(a.DadosAnterior)
						? a.DadosAnterior!
						: MontarMensagemAtividade(a.Operacao, a.TabelaNome, a.EntidadeId)),
				Ip = a.EnderecoIp ?? string.Empty
			})
			.ToList();

		return Ok(new PagedResponse<AuditTableLogDto>(logs, totalRegistros, pagina, tamanho));
	}

	private async Task<AdminDashboardStatsDto> MontarStatsAsync()
	{
		var totalAlunos = await _context.Usuario.CountAsync(u => u.Ativo && u.IdPerfil == (int)PerfilEnum.Aluno);
		var totalProfessores = await _context.Usuario.CountAsync(u => u.Ativo && u.IdPerfil == (int)PerfilEnum.Professor);
		var totalStaff = await _context.Usuario.CountAsync(u =>
			u.Ativo && (u.IdPerfil == (int)PerfilEnum.Administrador || u.IdPerfil == (int)PerfilEnum.Coodenador));

		// O schema atual não guarda histórico temporal de falhas; usamos o acumulado atual.
		var failedLogins24h = await _context.Usuario.SumAsync(u => u.TentativasFalhas ?? 0);

		var contasBloqueadas = await _context.Usuario.CountAsync(u =>
			u.BloqueadoAte.HasValue && u.BloqueadoAte.Value > DateTime.UtcNow);

		var dbStatus = await _context.Database.CanConnectAsync() ? "Conectado" : "Indisponivel";

		return new AdminDashboardStatsDto
		{
			TotalAlunos = totalAlunos,
			TotalProfessores = totalProfessores,
			TotalStaff = totalStaff,
			FailedLogins24h = failedLogins24h,
			ContasBloqueadas = contasBloqueadas,
			ServerStatus = "Online",
			DbStatus = dbStatus,
			DiskUsageApp = CalcularUsoDiscoAplicacao()
		};
	}

	private async Task<List<AdminRecentLogDto>> BuscarAtividadesRecentesAsync(int limit)
	{
		var registros = await _context.Auditorias
			.AsNoTracking()
			.ToListAsync();
		var itens = registros.OrderByDescending(a => a.DataHora).Take(limit).ToList();

		return itens.Select((x, indice) => new AdminRecentLogDto
		{
			Id = indice + 1,
			User = x.UsuarioId,
			Msg = MontarMensagemAtividade(x.Operacao, x.TabelaNome, x.EntidadeId),
			Time = FormatarTempoRelativo(x.DataHora.LocalDateTime)
		}).ToList();
	}

	private static int NormalizarLimiteLogs(int limit)
	{
		if (limit <= 0)
			return 10;

		return Math.Min(limit, 50);
	}

	private static int NormalizarPagina(int pagina)
	{
		return pagina <= 0 ? 1 : pagina;
	}

	private static int NormalizarTamanhoPagina(int tamanho)
	{
		if (tamanho <= 0)
			return 10;

		return Math.Min(tamanho, 50);
	}

	private static string MontarMensagemAtividade(string operacao, string tabelaNome, string entidadeId)
	{
		return $"{operacao} em {tabelaNome} (id: {entidadeId}).";
	}

	private static string FormatarTempoRelativo(DateTime data)
	{
		var delta = DateTime.Now - data;

		if (delta.TotalMinutes < 1)
			return "agora";

		if (delta.TotalHours < 1)
			return $"{Math.Floor(delta.TotalMinutes)} min atrás";

		if (delta.TotalDays < 1)
			return $"{Math.Floor(delta.TotalHours)} h atrás";

		return $"{Math.Floor(delta.TotalDays)} d atrás";
	}

	private static int CalcularUsoDiscoAplicacao()
	{
		try
		{
			var raiz = Path.GetPathRoot(AppContext.BaseDirectory);
			if (string.IsNullOrWhiteSpace(raiz))
				return 0;

			var drive = new DriveInfo(raiz);
			if (drive.TotalSize <= 0)
				return 0;

			var uso = 1 - ((double)drive.AvailableFreeSpace / drive.TotalSize);
			return (int)Math.Round(uso * 100, MidpointRounding.AwayFromZero);
		}
		catch
		{
			return 0;
		}
	}
}
