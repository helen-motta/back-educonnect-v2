namespace Modules.Academico.Application.DTOs;

public class AdminDashboardDto
{
    public AdminDashboardStatsDto Stats { get; set; } = new();
    public List<AdminRecentLogDto> RecentLogs { get; set; } = new();
}

public class AdminDashboardStatsDto
{
    public int TotalAlunos { get; set; }
    public int TotalProfessores { get; set; }
    public int TotalStaff { get; set; }
    public int FailedLogins24h { get; set; }
    public int ContasBloqueadas { get; set; }
    public string ServerStatus { get; set; } = "Online";
    public string DbStatus { get; set; } = "Indisponivel";
    public int DiskUsageApp { get; set; }
}

public class AdminRecentLogDto
{
    public int Id { get; set; }
    public string User { get; set; } = string.Empty;
    public string Msg { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
}
