using Microsoft.EntityFrameworkCore;
using Modules.Autenticacao.Domain.Entities;
using Modules.Academico.Domain.Entities;
using Shared.Domain.Entities;
using src.Modules.Academico.Domain.Entities;

namespace Shared.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuario { get; set; }
        
        // Academico
        public DbSet<Aluno> Alunos { get; set; }
        public DbSet<Disciplina> Disciplinas { get; set; }
        public DbSet<Matricula> Matriculas { get; set; }
        public DbSet<Avaliacao> Avaliacoes { get; set; }
        public DbSet<Frequencia> Frequencias { get; set; }
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<Requerimentos> Requerimentos { get; set; }
        public DbSet<Turma> Turmas { get; set; }
        public DbSet<GradeHorario> GradeHorarios { get; set; }

        public DbSet<InscricoesTurmas> InscricoesTurmas { get; set; }

        // Eventos
        public DbSet<Eventos> Eventos { get; set; }
        public DbSet<Auditoria> Auditorias { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Usuario>().ToTable("usuarios");
            
            // Mapear as propriedades para as colunas corretas
            modelBuilder.Entity<Usuario>()
                .Property(u => u.Id)
                .HasColumnName("id");
            
            modelBuilder.Entity<Usuario>()
                .Property(u => u.Nome)
                .HasColumnName("nome_completo");
            
            modelBuilder.Entity<Usuario>()
                .Property(u => u.Email)
                .HasColumnName("email");
            
            modelBuilder.Entity<Usuario>()
                .Property(u => u.SenhaHash)
                .HasColumnName("senha_hash");
            
            modelBuilder.Entity<Usuario>()
                .Property(u => u.IdPerfil)
                .HasColumnName("id_perfil");
            
            modelBuilder.Entity<Usuario>()
                .Property(u => u.Ativo)
                .HasColumnName("ativo");
            
            modelBuilder.Entity<Usuario>()
                .Property(u => u.CriadoEm)
                .HasColumnName("criado_em");
            
            modelBuilder.Entity<Usuario>()
                .Property(u => u.UltimoLogin)
                .HasColumnName("ultimo_login");
            
            modelBuilder.Entity<Usuario>()
                .Property(u => u.DataAceiteTermos)
                .HasColumnName("data_aceite_termos");
            
            modelBuilder.Entity<Usuario>()
                .Property(u => u.VersaoTermos)
                .HasColumnName("versao_termos");
            
            modelBuilder.Entity<Usuario>()
                .Property(u => u.TentativasFalhas)
                .HasColumnName("tentativas_falhas");
            
            modelBuilder.Entity<Usuario>()
                .Property(u => u.BloqueadoAte)
                .HasColumnName("bloqueado_ate");

            modelBuilder.Entity<Usuario>()
                .Property(u => u.ResetSenhaToken)
                .HasColumnName("reset_senha_token");

            modelBuilder.Entity<Usuario>()
                .Property(u => u.DataExpiraTokenResetSenha)
                .HasColumnName("data_expiracao_reset_senha_token");

            // Mapeamento Aluno
            modelBuilder.Entity<Aluno>().ToTable("alunos");
            modelBuilder.Entity<Aluno>()
                .Property(a => a.Id)
                .HasColumnName("id");
            modelBuilder.Entity<Aluno>()
                .Property(a => a.Nome)
                .HasColumnName("nome");
            modelBuilder.Entity<Aluno>()
                .Property(a => a.Matricula)
                .HasColumnName("matricula");
            modelBuilder.Entity<Aluno>()
                .Property(a => a.DataCadastro)
                .HasColumnName("data_cadastro");
            modelBuilder.Entity<Aluno>()
                .Property(a => a.Ativo)
                .HasColumnName("ativo");

            // Mapeamento Disciplina
            modelBuilder.Entity<Disciplina>().ToTable("disciplinas");
            modelBuilder.Entity<Disciplina>()
                .Property(d => d.Id)
                .HasColumnName("id");
            modelBuilder.Entity<Disciplina>()
                .Property(d => d.Nome)
                .HasColumnName("nome");
            modelBuilder.Entity<Disciplina>()
                .Property(d => d.Codigo)
                .HasColumnName("codigo");
            modelBuilder.Entity<Disciplina>()
                .Property(d => d.CargaHoraria)
                .HasColumnName("carga_horaria");

            // Mapeamento Avaliacao
            modelBuilder.Entity<Avaliacao>().ToTable("avaliacoes");
            modelBuilder.Entity<Avaliacao>()
                .Property(a => a.Id)
                .HasColumnName("id");
            modelBuilder.Entity<Avaliacao>()
                .Property(a => a.MatriculaId)
                .HasColumnName("matricula_id");
            modelBuilder.Entity<Avaliacao>()
                .Property(a => a.Nome)
                .HasColumnName("nome");
            modelBuilder.Entity<Avaliacao>()
                .Property(a => a.Fechada)
                .HasColumnName("fechada");

            // Mapeamento Frequencia
            modelBuilder.Entity<Frequencia>().ToTable("frequencias");
            modelBuilder.Entity<Frequencia>()
                .Property(f => f.Id)
                .HasColumnName("id");
            modelBuilder.Entity<Frequencia>()
                .Property(f => f.MatriculaId)
                .HasColumnName("matricula_id");
            modelBuilder.Entity<Frequencia>()
                .Property(f => f.AulasAssistidas)
                .HasColumnName("aulas_assistidas");
            modelBuilder.Entity<Frequencia>()
                .Property(f => f.TotalAulas)
                .HasColumnName("total_aulas");
            modelBuilder.Entity<Frequencia>()
                .Property(f => f.Consolidada)
                .HasColumnName("consolidada");

            // Mapeamento Turma
            modelBuilder.Entity<Turma>().ToTable("turmas");
            modelBuilder.Entity<Turma>()
                .Property(t => t.Id)
                .HasColumnName("id");
            modelBuilder.Entity<Turma>()
                .Property(t => t.NomeTurma)
                .HasColumnName("nome_turma");
            modelBuilder.Entity<Turma>()
                .Property(t => t.Sala)
                .HasColumnName("sala");
            modelBuilder.Entity<Turma>()
                .Property(t => t.Vagas)
                .HasColumnName("vagas");
            modelBuilder.Entity<Turma>()
                .Property(t => t.DisciplinaId)
                .HasColumnName("id_disciplina");
            modelBuilder.Entity<Turma>()
                .Property(t => t.ProfessorId)
                .HasColumnName("id_professor");

            // Mapeamento GradeHorario
            modelBuilder.Entity<GradeHorario>().ToTable("grade_horarios");
            modelBuilder.Entity<GradeHorario>()
                .Property(g => g.Codigo)
                .HasColumnName("codigo");
            modelBuilder.Entity<GradeHorario>()
                .Property(g => g.Inicio)
                .HasColumnName("inicio");
            modelBuilder.Entity<GradeHorario>()
                .Property(g => g.Fim)
                .HasColumnName("fim");

            // Eventos
    modelBuilder.Entity<Eventos>(entity =>
    {
        entity.ToTable("Eventos");
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Titulo).IsRequired().HasMaxLength(100);
        entity.Property(e => e.Descricao).HasMaxLength(500);

        entity.HasOne<Usuario>() 
              .WithMany() 
              .HasForeignKey(e => e.ProfessorId)
              .OnDelete(DeleteBehavior.Restrict); 

        entity.HasOne<Disciplina>()
              .WithMany()
              .HasForeignKey(e => e.DisciplinaId)
              .OnDelete(DeleteBehavior.Cascade);
    });

        modelBuilder.Entity<Auditoria>(entity =>
        {
          entity.ToTable("auditoria");
          entity.HasKey(a => a.Id);

          entity.Property(a => a.Id)
              .HasColumnName("id")
              .HasDefaultValueSql("NEWID()");

          entity.Property(a => a.TabelaNome)
              .HasColumnName("tabela_nome")
              .HasMaxLength(100)
              .IsRequired();

          entity.Property(a => a.EntidadeId)
              .HasColumnName("entidade_id")
              .HasMaxLength(50)
              .IsRequired();

          entity.Property(a => a.Operacao)
              .HasColumnName("operacao")
              .HasMaxLength(10)
              .IsRequired();

          entity.Property(a => a.DadosAnterior)
              .HasColumnName("dados_anterior");

          entity.Property(a => a.DadosAtual)
              .HasColumnName("dados_atual");

          entity.Property(a => a.UsuarioId)
              .HasColumnName("usuario_id")
              .HasMaxLength(50)
              .IsRequired();

          entity.Property(a => a.DataHora)
              .HasColumnName("data_hora")
              .HasDefaultValueSql("SYSDATETIMEOFFSET()");

          entity.Property(a => a.EnderecoIp)
              .HasColumnName("endereco_ip")
              .HasMaxLength(45);

          entity.Property(a => a.UserAgent)
              .HasColumnName("user_agent")
              .HasMaxLength(255);
        });
}
    }
}