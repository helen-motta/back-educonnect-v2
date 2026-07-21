# EduConnect API

API responsável por centralizar as operações da faculdade: usuários, cursos, disciplinas, turmas, matrículas, notas, frequência, atividades, comunicados, requerimentos, documentos e dashboards.

Também oferece auditoria persistida, logs operacionais, autenticação JWT e armazenamento de imagens no Amazon S3. O projeto contempla também Testes Unitários.

## Arquitetura

O backend utiliza uma arquitetura modular em camadas:

```text
src/
├── Api/             inicialização e middlewares
├── Modules/
│   ├── Autenticacao/
│   └── Academico/
└── Shared/          banco, AWS e recursos compartilhados
```

Cada módulo é dividido em:

- **Api:** controllers e contratos HTTP;
- **Application:** DTOs e casos de uso;
- **Domain:** entidades, regras e interfaces;
- **Infrastructure:** repositórios e integrações.

O fluxo principal é:

```text
Controller → Caso de uso → Repositório → Banco/serviço externo
```

## Funcionalidades

- autenticação e recuperação de senha;
- gestão de usuários e perfis;
- cursos, disciplinas, turmas e salas;
- matrículas e inscrições;
- avaliações, notas e frequência;
- atividades e correção de entregas;
- comunicados e calendário;
- boletim e horários;
- requerimentos administrativos;
- dashboards por perfil;
- documentos PDF;
- configurações do portal.

## Logs e auditoria

O projeto possui dois tipos de log:

- **operacional:** enviado ao console pelo ASP.NET Core;
- **auditoria:** persistido no banco.

A auditoria armazena:

- usuário;
- operação;
- entidade alterada;
- valores anteriores e posteriores;
- data e hora;
- IP;
- User-Agent.

Consultas:

```text
GET /api/audit/dashboard
GET /api/audit/logs
GET /api/audit/dashboard/stats
GET /api/audit/dashboard/recent-logs
```


## Amazon S3

Fotos de perfil são enviadas ao backend:

```text
POST /api/perfil/foto
```

A API valida o arquivo e o armazena no S3 usando chaves como:

```text
usuarios/2026/07/<guid>.jpg
```

São aceitos JPEG, PNG, WebP e GIF, com limite de 5 MB.

As credenciais nunca são enviadas ao frontend. Em produção, configure:

```text
AWS__Region
AWS__BucketName
AWS__AccessKey
AWS__SecretKey
AWS__PublicBaseUrl
```

## Banco de dados

O ambiente local utiliza SQLite e cria automaticamente:

```text
data/educonnect.db
```

Na primeira execução, o backend monta o esquema e adiciona dados de demonstração. Não é necessário clonar um banco.

Produção pode utilizar SQL Server:

```text
Database__Provider=SqlServer
ConnectionStrings__DefaultConnection=<connection-string>
```

## Tecnologias

- .NET 10;
- ASP.NET Core;
- Entity Framework Core;
- SQLite e SQL Server;
- JWT Bearer;
- BCrypt;
- Amazon S3;
- SendGrid;
- Swagger;
- PdfSharp;
- Docker.

## Execução

```powershell
dotnet restore
dotnet run --urls http://localhost:5055
```

```text
API:     http://localhost:5055
Swagger: http://localhost:5055/swagger
Health:  http://localhost:5055/health
```

Contas locais, senha `123456`:

```text
admin@educonnect.local
coordenador@educonnect.local
professor@educonnect.local
aluno@educonnect.local
```

Testes:
```
dotnet test
```