using System.ComponentModel.DataAnnotations.Schema;

namespace Modules.Academico.Domain.Entities;

[Table("portal_configuracoes")]
public sealed class PortalConfiguracao
{
    [Column("id")]
    public int Id { get; set; } = 1;
    [Column("feature_dark_mode")]
    public bool FeatureDarkMode { get; set; } = true;
    [Column("feature_carteirinha")]
    public bool FeatureCarteirinha { get; set; } = true;
    [Column("feature_matricula")]
    public bool FeatureMatricula { get; set; } = true;
    [Column("feature_financeiro")]
    public bool FeatureFinanceiro { get; set; }
}
