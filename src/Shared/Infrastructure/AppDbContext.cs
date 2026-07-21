using Microsoft.EntityFrameworkCore;
using Modules.Academico.Domain.Entities;
using Modules.Autenticacao.Domain.Entities;
using Shared.Domain.Entities;
using src.Modules.Academico.Domain.Entities;

namespace Shared.Infrastructure;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuario => Set<Usuario>();
    public DbSet<Aluno> Alunos => Set<Aluno>();
    public DbSet<Disciplina> Disciplinas => Set<Disciplina>();
    public DbSet<Matricula> Matriculas => Set<Matricula>();
    public DbSet<AvaliacaoProfessor> AvaliacoesProfessor => Set<AvaliacaoProfessor>();
    public DbSet<FrequenciaProfessor> FrequenciasProfessor => Set<FrequenciaProfessor>();
    public DbSet<NotaProfessor> NotasProfessor => Set<NotaProfessor>();
    public DbSet<Curso> Cursos => Set<Curso>();
    public DbSet<Requerimentos> Requerimentos => Set<Requerimentos>();
    public DbSet<Turma> Turmas => Set<Turma>();
    public DbSet<TurmaSlot> TurmaSlots => Set<TurmaSlot>();
    public DbSet<InscricoesTurmas> InscricoesTurmas => Set<InscricoesTurmas>();
    public DbSet<Eventos> Eventos => Set<Eventos>();
    public DbSet<Auditoria> Auditorias => Set<Auditoria>();
    public DbSet<PortalConfiguracao> PortalConfiguracoes => Set<PortalConfiguracao>();
    public DbSet<Comunicado> Comunicados => Set<Comunicado>();
    public DbSet<ComunicadoTurma> ComunicadosTurmas => Set<ComunicadoTurma>();
    public DbSet<Atividade> Atividades => Set<Atividade>();
    public DbSet<EntregaAtividade> EntregasAtividades => Set<EntregaAtividade>();
    public DbSet<SolicitacaoMatricula> SolicitacoesMatricula => Set<SolicitacaoMatricula>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuarios");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Email).IsUnique();
            entity.HasIndex(x => x.Registro).IsUnique();
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.Nome).HasColumnName("nome_completo").HasMaxLength(150);
            entity.Property(x => x.Email).HasColumnName("email").HasMaxLength(150);
            entity.Property(x => x.SenhaHash).HasColumnName("senha_hash");
            entity.Property(x => x.IdPerfil).HasColumnName("id_perfil");
            entity.Property(x => x.Ativo).HasColumnName("ativo");
            entity.Property(x => x.CriadoEm).HasColumnName("criado_em");
            entity.Property(x => x.UltimoLogin).HasColumnName("ultimo_login");
            entity.Property(x => x.DataAceiteTermos).HasColumnName("data_aceite_termos");
            entity.Property(x => x.VersaoTermos).HasColumnName("versao_termos");
            entity.Property(x => x.TentativasFalhas).HasColumnName("tentativas_falhas");
            entity.Property(x => x.BloqueadoAte).HasColumnName("bloqueado_ate");
            entity.Property(x => x.ResetSenhaToken).HasColumnName("reset_senha_token");
            entity.Property(x => x.DataExpiraTokenResetSenha).HasColumnName("data_expiracao_reset_senha_token");
            entity.Property(x => x.Registro).HasColumnName("registro");
            entity.Property(x => x.Cep).HasColumnName("cep");
            entity.Property(x => x.Endereco).HasColumnName("endereco");
            entity.Property(x => x.Numero).HasColumnName("numero");
            entity.Property(x => x.Complemento).HasColumnName("complemento");
            entity.Property(x => x.Bairro).HasColumnName("bairro");
            entity.Property(x => x.Cidade).HasColumnName("cidade");
            entity.Property(x => x.Estado).HasColumnName("estado");
            entity.Property(x => x.Telefone).HasColumnName("telefone");
            entity.Property(x => x.Cpf).HasColumnName("cpf");
            entity.Property(x => x.Rg).HasColumnName("rg");
            entity.Property(x => x.FotoUrl).HasColumnName("foto_url");
            entity.Property(x => x.NotificarTarefas).HasColumnName("notificar_tarefas");
            entity.Property(x => x.NotificarAvisos).HasColumnName("notificar_avisos");
            entity.Property(x => x.NotificarNotas).HasColumnName("notificar_notas");
        });

        modelBuilder.Entity<Aluno>(entity =>
        {
            entity.ToTable("alunos");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
            entity.Property(x => x.Nome).HasColumnName("nome");
            entity.Property(x => x.Matricula).HasColumnName("matricula");
            entity.Property(x => x.DataCadastro).HasColumnName("data_cadastro");
            entity.Property(x => x.Ativo).HasColumnName("ativo");
        });

        modelBuilder.Entity<Curso>(entity =>
        {
            entity.ToTable("cursos");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Codigo).IsUnique();
            entity.HasOne(x => x.Coordenador).WithMany().HasForeignKey(x => x.IdCoordenador).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Disciplina>(entity =>
        {
            entity.ToTable("disciplinas");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Codigo).IsUnique();
            entity.HasOne(x => x.Curso).WithMany(x => x.Disciplinas).HasForeignKey(x => x.IdCurso).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Turma>(entity =>
        {
            entity.ToTable("turmas");
            entity.HasKey(x => x.Id);
            entity.HasOne(x => x.Disciplina).WithMany().HasForeignKey(x => x.DisciplinaId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Professor).WithMany().HasForeignKey(x => x.ProfessorId).OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(x => x.TurmaSlots).WithOne(x => x.Turma).HasForeignKey(x => x.TurmaId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TurmaSlot>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Ignore(x => x.DiaSemanaNome);
            entity.Ignore(x => x.Horario);
        });

        modelBuilder.Entity<InscricoesTurmas>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.AlunoId, x.TurmaId }).IsUnique();
            entity.HasOne(x => x.Turma).WithMany(x => x.InscricoesTurmas).HasForeignKey(x => x.TurmaId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Aluno).WithMany().HasForeignKey(x => x.AlunoId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Matricula>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasOne(x => x.Turma).WithMany().HasForeignKey(x => x.TurmaId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Aluno).WithMany().HasForeignKey(x => x.AlunoId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Requerimentos>(entity =>
        {
            entity.ToTable("requerimentos");
            entity.HasKey(x => x.Id);
            entity.HasOne(x => x.Usuario).WithMany().HasForeignKey(x => x.IdUsuario).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Eventos>(entity =>
        {
            entity.ToTable("eventos");
            entity.HasKey(x => x.Id);
            entity.HasOne<Usuario>().WithMany().HasForeignKey(x => x.ProfessorId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<Disciplina>().WithMany().HasForeignKey(x => x.DisciplinaId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<AvaliacaoProfessor>().HasKey(x => x.Id);
        modelBuilder.Entity<FrequenciaProfessor>().HasKey(x => x.Id);
        modelBuilder.Entity<NotaProfessor>().HasKey(x => x.Id);
        modelBuilder.Entity<PortalConfiguracao>().HasKey(x => x.Id);
        modelBuilder.Entity<SolicitacaoMatricula>().HasKey(x => x.Id);

        modelBuilder.Entity<Auditoria>(entity =>
        {
            entity.ToTable("auditoria");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
            entity.Property(x => x.TabelaNome).HasColumnName("tabela_nome").HasMaxLength(100);
            entity.Property(x => x.EntidadeId).HasColumnName("entidade_id").HasMaxLength(50);
            entity.Property(x => x.Operacao).HasColumnName("operacao").HasMaxLength(20);
            entity.Property(x => x.DadosAnterior).HasColumnName("dados_anterior");
            entity.Property(x => x.DadosAtual).HasColumnName("dados_atual");
            entity.Property(x => x.UsuarioId).HasColumnName("usuario_id").HasMaxLength(50);
            entity.Property(x => x.DataHora).HasColumnName("data_hora");
            entity.Property(x => x.EnderecoIp).HasColumnName("endereco_ip").HasMaxLength(45);
            entity.Property(x => x.UserAgent).HasColumnName("user_agent").HasMaxLength(255);
        });

        modelBuilder.Entity<Comunicado>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasOne(x => x.Professor).WithMany().HasForeignKey(x => x.ProfessorId).OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<ComunicadoTurma>(entity =>
        {
            entity.HasKey(x => new { x.ComunicadoId, x.TurmaId });
            entity.HasOne(x => x.Comunicado).WithMany(x => x.Turmas).HasForeignKey(x => x.ComunicadoId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Turma).WithMany().HasForeignKey(x => x.TurmaId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Atividade>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasOne(x => x.Turma).WithMany().HasForeignKey(x => x.TurmaId).OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<EntregaAtividade>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.AtividadeId, x.AlunoId }).IsUnique();
            entity.HasOne(x => x.Atividade).WithMany(x => x.Entregas).HasForeignKey(x => x.AtividadeId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Aluno).WithMany().HasForeignKey(x => x.AlunoId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
