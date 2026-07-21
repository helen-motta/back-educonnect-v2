using Microsoft.EntityFrameworkCore;
using Modules.Academico.Domain.Entities;
using Modules.Autenticacao.Domain.Entities;
using Shared.Infrastructure;
using src.Modules.Academico.Domain.Entities;

public static class DemoDataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Usuario.AnyAsync())
            return;

        var passwordHash = BCrypt.Net.BCrypt.HashPassword("123456");
        var users = new[]
        {
            CreateUser(1, "Administrador EduConnect", "admin@educonnect.local", "ADM0001", 1, passwordHash),
            CreateUser(2, "Carla Coordenadora", "coordenador@educonnect.local", "COO0001", 2, passwordHash),
            CreateUser(3, "Paulo Professor", "professor@educonnect.local", "PROF0001", 3, passwordHash),
            CreateUser(4, "Ana Souza", "aluno@educonnect.local", "20260001", 4, passwordHash),
            CreateUser(5, "Bruno Mendes", "bruno@educonnect.local", "20260002", 4, passwordHash),
            CreateUser(6, "Carla Nunes", "carla@educonnect.local", "20260003", 4, passwordHash)
        };
        await context.Usuario.AddRangeAsync(users);

        var students = new[]
        {
            new Aluno(4, "Ana Souza", "20260001"),
            new Aluno(5, "Bruno Mendes", "20260002"),
            new Aluno(6, "Carla Nunes", "20260003")
        };
        await context.Alunos.AddRangeAsync(students);

        var course = new Curso
        {
            Id = 1, Nome = "Sistemas de Informação", Codigo = "SI", Descricao = "Curso de tecnologia e negócios",
            CargaHoraria = 3200, Modalidade = 1, IdCoordenador = 2, Ativo = true
        };
        await context.Cursos.AddAsync(course);

        var subjects = new[]
        {
            new Disciplina { Id = 1, IdCurso = 1, Nome = "Engenharia de Software", Codigo = "ES101", Ementa = "Processos e requisitos", CargaHoraria = 80, Creditos = 4, SemestreIdeal = 3, Ativo = true },
            new Disciplina { Id = 2, IdCurso = 1, Nome = "Cálculo I", Codigo = "CAL101", Ementa = "Limites e derivadas", CargaHoraria = 80, Creditos = 4, SemestreIdeal = 1, Ativo = true },
            new Disciplina { Id = 3, IdCurso = 1, Nome = "Banco de Dados", Codigo = "BD201", Ementa = "Modelagem e SQL", CargaHoraria = 80, Creditos = 4, SemestreIdeal = 3, Ativo = true }
        };
        await context.Disciplinas.AddRangeAsync(subjects);

        var classes = new[]
        {
            new Turma { Id = 1, NomeTurma = "ES - Turma A", Sala = "A1-01", Vagas = 35, DisciplinaId = 1, ProfessorId = 3 },
            new Turma { Id = 2, NomeTurma = "Cálculo - Turma A", Sala = "B1-02", Vagas = 35, DisciplinaId = 2, ProfessorId = 3 },
            new Turma { Id = 3, NomeTurma = "BD - Turma B", Sala = "LAB-03", Vagas = 25, DisciplinaId = 3, ProfessorId = 3 }
        };
        await context.Turmas.AddRangeAsync(classes);
        await context.TurmaSlots.AddRangeAsync(
            new TurmaSlot { Id = 1, TurmaId = 1, CodigoSlot = "N1", DiaSemana = 2 },
            new TurmaSlot { Id = 2, TurmaId = 1, CodigoSlot = "N1", DiaSemana = 4 },
            new TurmaSlot { Id = 3, TurmaId = 2, CodigoSlot = "M1", DiaSemana = 3 },
            new TurmaSlot { Id = 4, TurmaId = 2, CodigoSlot = "M1", DiaSemana = 5 },
            new TurmaSlot { Id = 5, TurmaId = 3, CodigoSlot = "T2", DiaSemana = 2 },
            new TurmaSlot { Id = 6, TurmaId = 3, CodigoSlot = "T2", DiaSemana = 6 });

        await context.InscricoesTurmas.AddRangeAsync(
            Enrollment(1, 4, 1, 8.5m, 7.5m, 9m, 8.3m, 92),
            Enrollment(2, 5, 1, 7m, 6.5m, 8m, 7.1m, 81),
            Enrollment(3, 6, 1, 9m, 8.5m, 9.5m, 9m, 96),
            Enrollment(4, 4, 2, 7.5m, 8m, 8m, 7.8m, 88),
            Enrollment(5, 5, 2, 5m, 6m, 7m, 5.8m, 73),
            Enrollment(6, 4, 3, 9m, 9m, 10m, 9.3m, 98));

        await context.Matriculas.AddRangeAsync(
            Matriculation(1, 4, 1, 1), Matriculation(2, 5, 1, 1), Matriculation(3, 6, 1, 1),
            Matriculation(4, 4, 2, 2), Matriculation(5, 5, 2, 2), Matriculation(6, 4, 3, 3));

        await context.AvaliacoesProfessor.AddRangeAsync(
            new AvaliacaoProfessor { Id = 1, IdTurma = 1, Nome = "P1", DataPrevista = DateTime.UtcNow.AddDays(7), Peso = 0.4m },
            new AvaliacaoProfessor { Id = 2, IdTurma = 1, Nome = "Projeto", DataPrevista = DateTime.UtcNow.AddDays(21), Peso = 0.6m },
            new AvaliacaoProfessor { Id = 3, IdTurma = 2, Nome = "P1", DataPrevista = DateTime.UtcNow.AddDays(10), Peso = 1m },
            new AvaliacaoProfessor { Id = 4, IdTurma = 3, Nome = "Trabalho SQL", DataPrevista = DateTime.UtcNow.AddDays(14), Peso = 1m });
        await context.NotasProfessor.AddRangeAsync(
            new NotaProfessor { Id = 1, IdAvaliacao = 1, IdMatricula = 1, ValorObtido = 8.5m },
            new NotaProfessor { Id = 2, IdAvaliacao = 1, IdMatricula = 2, ValorObtido = 7m });
        await context.FrequenciasProfessor.AddRangeAsync(
            new FrequenciaProfessor { Id = 1, IdMatricula = 1, DataAula = DateTime.UtcNow.AddDays(-3), Presente = true, QtdAulas = 2 },
            new FrequenciaProfessor { Id = 2, IdMatricula = 2, DataAula = DateTime.UtcNow.AddDays(-3), Presente = false, QtdAulas = 2 });

        await context.Requerimentos.AddRangeAsync(
            new Requerimentos { Id = 1, IdUsuario = 4, TipoSolicitacao = "Aproveitamento de disciplina", Status = "Pendente", Observacao = "Análise de equivalência" },
            new Requerimentos { Id = 2, IdUsuario = 5, TipoSolicitacao = "Ajuste de matrícula", Status = "Pendente", Observacao = "Inclusão de disciplina" });
        await context.Eventos.AddRangeAsync(
            new Eventos("P1 de Engenharia de Software", DateTime.UtcNow.AddDays(7), "Avaliação presencial", TipoEvento.Disciplina, 3, 1),
            new Eventos("Workshop de Carreira", DateTime.UtcNow.AddDays(15), "Evento aberto aos alunos", TipoEvento.Workshop, 3));
        await context.PortalConfiguracoes.AddAsync(new PortalConfiguracao());

        var notice = new Comunicado { Id = 1, ProfessorId = 3, Assunto = "Boas-vindas ao semestre", Mensagem = "O material inicial já está disponível.", CriadoEm = DateTime.UtcNow.AddDays(-2) };
        notice.Turmas.Add(new ComunicadoTurma { ComunicadoId = 1, TurmaId = 1 });
        await context.Comunicados.AddAsync(notice);

        await context.Atividades.AddRangeAsync(
            new Atividade { Id = 1, TurmaId = 1, Titulo = "Levantamento de requisitos", Descricao = "Produzir o documento de requisitos do projeto.", Tipo = "Trabalho", Prazo = DateTime.UtcNow.AddDays(12), Pontuacao = 10, Status = "aberta" },
            new Atividade { Id = 2, TurmaId = 2, Titulo = "Lista de derivadas", Descricao = "Resolver a lista publicada no portal.", Tipo = "Exercício", Prazo = DateTime.UtcNow.AddDays(8), Pontuacao = 5, Status = "aberta" });
        await context.EntregasAtividades.AddAsync(new EntregaAtividade
        {
            Id = 1, AtividadeId = 1, AlunoId = 4, ArquivoNome = "requisitos-ana.png", TipoArquivo = "image",
            ArquivoUrl = "https://educonnect-imagens-dev.s3.sa-east-1.amazonaws.com/demo/requisitos-ana.png", EnviadoEm = DateTime.UtcNow.AddDays(-1)
        });

        await context.SaveChangesAsync();
    }

    private static Usuario CreateUser(int id, string name, string email, string registration, int profile, string passwordHash) => new()
    {
        Id = id, Nome = name, Email = email, Registro = registration, IdPerfil = profile, SenhaHash = passwordHash,
        Ativo = true, Cep = "01001-000", Endereco = "Praça da Sé", Numero = "100", Bairro = "Sé",
        Cidade = "São Paulo", Estado = "SP", Telefone = "(11) 99999-0000", Cpf = $"0000000000{id}", Rg = $"0000000{id}"
    };

    private static InscricoesTurmas Enrollment(int id, int studentId, int classId, decimal p1, decimal p2, decimal work, decimal finalGrade, int attendance) => new()
    {
        Id = id, AlunoId = studentId, TurmaId = classId, P1 = p1, P2 = p2, Trabalho = work,
        NotaFinal = finalGrade, Frequencia = attendance, Status = "Ativo"
    };

    private static Matricula Matriculation(int id, int studentId, int classId, int subjectId) => new()
    {
        Id = id, AlunoId = studentId, TurmaId = classId, DisciplinaId = subjectId, PeriodoLetivo = $"{DateTime.UtcNow.Year}.2"
    };
}
