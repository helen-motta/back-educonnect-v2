# EduConnect API

Backend central do EduConnect, responsável por concentrar autenticação, gestão acadêmica, comunicação, acompanhamento de alunos, auditoria, documentos e integrações externas em uma única API.

O sistema foi pensado como o núcleo digital da faculdade. Administradores, coordenadores, professores e alunos utilizam a mesma base de dados e os mesmos fluxos de negócio, com permissões e interfaces diferentes para cada perfil.

Entre as principais responsabilidades estão:

- autenticação e gerenciamento de usuários;
- cursos, disciplinas, turmas e salas;
- matrículas e inscrições;
- notas, avaliações e frequência;
- atividades, entregas e correções;
- comunicados e calendário acadêmico;
- requerimentos administrativos;
- dashboards por perfil;
- geração de documentos;
- armazenamento de imagens no Amazon S3;
- logs operacionais e auditoria persistida;
- configurações e funcionalidades do portal.

## Visão geral da arquitetura

O backend utiliza ASP.NET Core e segue uma arquitetura modular em camadas:

```text
src/
├── Api/
│   ├── Program.cs
│   ├── DatabaseInitializer.cs
│   ├── DemoDataSeeder.cs
│   └── Middlewares/
│
├── Modules/
│   ├── Autenticacao/
│   │   ├── Api/
│   │   ├── Application/
│   │   ├── Domain/
│   │   └── Infrastructure/
│   │
│   └── Academico/
│       ├── Api/
│       ├── Application/
│       ├── Domain/
│       └── Infrastructure/
│
└── Shared/
    ├── Application/
    ├── Domain/
    └── Infrastructure/
```

Cada camada possui uma responsabilidade definida:

- **Api:** controllers, autenticação HTTP, validação de entrada e códigos de resposta.
- **Application:** casos de uso, DTOs e coordenação dos fluxos.
- **Domain:** entidades, regras acadêmicas, interfaces e contratos.
- **Infrastructure:** Entity Framework, repositórios, AWS, e-mail e serviços externos.
- **Shared:** recursos reutilizados por diferentes módulos.

### Decisão arquitetural

A separação por módulos evita que autenticação, regras acadêmicas e infraestrutura fiquem misturadas em controllers.

O fluxo esperado é:

```text
Controller
    ↓
Caso de uso
    ↓
Interface de repositório ou serviço
    ↓
Implementação de infraestrutura
    ↓
Banco de dados ou integração externa
```

Isso permite substituir SQLite por SQL Server, S3 por outra implementação de storage ou SendGrid por outro provedor sem reescrever as regras acadêmicas.

## Funções acadêmicas centralizadas

O backend centraliza as principais operações da faculdade.

### Administração

- cadastro, consulta e atualização de usuários;
- desativação lógica de usuários;
- acompanhamento de alunos, professores e funcionários;
- painel administrativo;
- consulta de logs e auditorias;
- monitoramento do banco e da aplicação;
- configurações gerais e feature flags do portal.

### Coordenação

- criação e atualização de cursos;
- desativação lógica de cursos;
- cadastro de disciplinas;
- criação e organização de turmas;
- acompanhamento de requerimentos;
- análise de solicitações acadêmicas;
- visualização de indicadores gerais;
- configuração de funcionalidades disponíveis no portal.

### Professores

- consulta das próprias turmas;
- criação de avaliações;
- lançamento de notas;
- lançamento e consulta de frequência;
- criação e manutenção de atividades;
- consulta e correção de entregas;
- publicação de comunicados;
- manutenção de eventos acadêmicos;
- consulta de salas e horários.

### Alunos

- consulta de boletim;
- consulta de frequência;
- visualização de horários;
- abertura e acompanhamento de requerimentos;
- solicitação de matrícula;
- consulta de cursos disponíveis;
- perfil e preferências;
- carteirinha virtual;
- recuperação de senha.

### Serviços institucionais

- geração de documentos em PDF;
- recuperação de senha por e-mail;
- dashboard por perfil;
- health check da aplicação;
- documentação Swagger;
- armazenamento externo de imagens;
- auditoria de alterações acadêmicas.

## Sistema de logs e auditoria

O projeto trabalha com duas categorias de log.

### Logs operacionais

Os logs operacionais utilizam o sistema de logging do ASP.NET Core.

```csharp
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
```

Esses registros são exibidos no terminal e incluem:

- inicialização da aplicação;
- falhas de configuração;
- erros de banco;
- exceções não tratadas;
- requisições com falha;
- mensagens do serviço de e-mail local.

Em desenvolvimento, o terminal também recebe o conteúdo dos e-mails de recuperação de senha, eliminando a necessidade de configurar um serviço externo apenas para executar o projeto.

### Auditoria persistida

As alterações relevantes podem gerar registros permanentes na tabela de auditoria.

Cada registro possui:

- identificador único;
- tabela ou domínio afetado;
- identificador da entidade;
- operação realizada;
- dados anteriores em JSON;
- dados posteriores em JSON;
- usuário responsável;
- data e hora;
- endereço IP;
- User-Agent da requisição.

Exemplo conceitual:

```json
{
  "tabelaNome": "cursos",
  "entidadeId": "12",
  "operacao": "UPDATE",
  "dadosAnterior": {},
  "dadosAtual": {},
  "usuarioId": "2",
  "dataHora": "2026-07-21T15:00:00Z",
  "enderecoIp": "127.0.0.1",
  "userAgent": "Mozilla/5.0"
}
```

Os registros podem ser consultados por meio de:

```text
GET /api/audit/dashboard
GET /api/audit/dashboard/stats
GET /api/audit/dashboard/recent-logs
GET /api/audit/logs
```

A listagem aceita paginação e filtros por usuário, tipo, ação e período.

### Estado atual da cobertura

A infraestrutura de auditoria está implementada e operações como atualização de cursos e criação de disciplinas já geram registros persistidos.

A auditoria ainda não é automática para todos os endpoints. Para cobertura institucional completa, o próximo passo recomendado é adotar um filtro global, interceptor do Entity Framework ou eventos de domínio para registrar automaticamente todas as operações críticas.

## Armazenamento de imagens no Amazon S3

As imagens relacionadas ao backend não são armazenadas no banco nem no sistema de arquivos da API.

O fluxo adotado é:

```text
Frontend
    ↓ multipart/form-data
Backend
    ↓ validação
Amazon S3
    ↓
URL pública
    ↓
Banco de dados
```

O upload de foto de perfil utiliza:

```text
POST /api/perfil/foto
```

O backend valida:

- tamanho máximo de 5 MB;
- JPEG;
- PNG;
- WebP;
- GIF.

A extensão final é definida pelo tipo MIME validado, evitando confiar apenas no nome enviado pelo usuário.

As chaves do S3 seguem o padrão:

```text
usuarios/AAAA/MM/<guid>.<extensão>
```

Exemplo:

```text
usuarios/2026/07/2f5d0e48ce884d3281a5d98eefccb824.jpg
```

Configuração:

```json
{
  "AWS": {
    "Region": "sa-east-1",
    "BucketName": "educonnect-imagens-dev",
    "AccessKey": "CHAVE_DE_DESENVOLVIMENTO",
    "SecretKey": "SEGREDO_DE_DESENVOLVIMENTO",
    "PublicBaseUrl": "https://educonnect-imagens-dev.s3.sa-east-1.amazonaws.com"
  }
}
```

As credenciais versionadas são fictícias. Em ambientes reais, utilize variáveis de ambiente, AWS Secrets Manager ou outro gerenciador de segredos:

```text
AWS__Region=sa-east-1
AWS__BucketName=educonnect-imagens-prod
AWS__AccessKey=<access-key>
AWS__SecretKey=<secret-key>
AWS__PublicBaseUrl=https://cdn.exemplo.com
```

O usuário IAM precisa, no mínimo, de permissão `s3:PutObject` no prefixo utilizado pela aplicação.

O frontend nunca recebe credenciais AWS. Ele envia a imagem para a API e recebe apenas a URL final.

## Banco de dados sem dependência de clone

Para desenvolvimento, o banco padrão é SQLite:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=data/educonnect.db"
  },
  "Database": {
    "Provider": "Sqlite",
    "Initialize": true,
    "SeedDemoData": true
  }
}
```

Na primeira execução, a aplicação:

1. cria a pasta `data`;
2. cria `educonnect.db`;
3. monta o esquema com `EnsureCreatedAsync`;
4. adiciona dados de demonstração;
5. disponibiliza contas para todos os perfis.

Assim, nenhum banco precisa ser baixado ou restaurado para iniciar o projeto.

Para recriar a base local:

1. encerre a API;
2. remova `data/educonnect.db`;
3. inicie a API novamente.

### Decisão técnica

`EnsureCreatedAsync` foi escolhido para simplificar o onboarding e os testes locais.

Em produção, recomenda-se:

- `Database__Initialize=false`;
- migrations versionadas;
- SQL Server;
- aplicação das migrations por pipeline controlado.

### SQL Server

O projeto também possui suporte ao SQL Server por meio do Entity Framework Core.

```text
Database__Provider=SqlServer
ConnectionStrings__DefaultConnection=<connection-string>
```

O arquivo `appsettings.Production.example.json` contém um modelo de configuração para produção.

## Dados de demonstração

Todas as contas locais utilizam a senha:

```text
123456
```

| Perfil | E-mail |
| --- | --- |
| Administrador | `admin@educonnect.local` |
| Coordenador | `coordenador@educonnect.local` |
| Professor | `professor@educonnect.local` |
| Aluno | `aluno@educonnect.local` |

Os dados são inseridos somente quando:

```text
Database__SeedDemoData=true
```

## Autenticação e segurança

A autenticação utiliza JWT Bearer.

O token contém informações como:

- identificador do usuário;
- e-mail;
- perfil;
- nome.

As senhas são protegidas com BCrypt.

Outras medidas implementadas:

- validação de assinatura JWT;
- validação de emissor e audiência;
- expiração de token;
- CORS configurável;
- limite de tamanho para upload;
- validação de tipos de imagem;
- tratamento centralizado de exceções;
- desativação lógica de usuários e cursos;
- secrets substituíveis por variáveis de ambiente.

Configuração local:

```json
{
  "JwtSettings": {
    "SecretKey": "educonnect-development-key-change-before-production-2026",
    "Issuer": "EduConnect",
    "Audience": "EduConnectUsers",
    "ExpirationMinutes": 120
  }
}
```

A chave local deve ser substituída em produção.

## E-mail e recuperação de senha

O projeto possui duas estratégias.

### Desenvolvimento

```text
Email__Provider=Console
```

O e-mail é exibido no terminal, permitindo testar recuperação de senha sem conta externa.

### Produção

```text
Email__Provider=SendGrid
SendGrid__ApiKey=<api-key>
SendGrid__FromEmail=nao-responda@exemplo.com
SendGrid__FromName=EduConnect
```

A escolha do provedor é feita por configuração e não altera o caso de uso de autenticação.

## Documentos PDF

O backend possui suporte à geração de documentos acadêmicos em PDF.

Rota principal:

```text
GET /api/documentos/gerar-pdf/{tipo}
```

O projeto inclui PdfSharp e QuestPDF. A geração implementada atualmente utiliza PdfSharp.

Essa funcionalidade pode ser utilizada para:

- atestados;
- declarações;
- documentos de matrícula;
- comprovantes acadêmicos.

## Tecnologias

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core 10
- SQLite
- SQL Server
- JWT Bearer
- BCrypt
- Amazon S3 SDK
- SendGrid
- PdfSharp
- QuestPDF
- Swagger / OpenAPI
- Docker

## Rotas principais

### Autenticação

```text
POST /api/auth/login
POST /api/auth/esqueci-senha
POST /api/auth/reset-senha
```

### Usuários e perfil

```text
GET    /api/usuarios
POST   /api/usuarios
GET    /api/usuarios/{id}
PUT    /api/usuarios/{id}
PUT    /api/usuarios/desativar/{id}
GET    /api/perfil
POST   /api/perfil/foto
PUT    /api/perfil/preferencias
```

### Estrutura acadêmica

```text
GET    /api/cursos
POST   /api/cursos
PUT    /api/cursos/{id}
DELETE /api/cursos/{id}

GET  /api/disciplinas/curso/{idCurso}
POST /api/disciplinas

GET    /api/turmas
GET    /api/turmas/{id}
GET    /api/turmas/professor
GET    /api/turmas/aluno/{id}/horarios
POST   /api/turmas
PUT    /api/turmas/{id}
DELETE /api/turmas/{id}
```

### Professores

```text
POST /api/professor/avaliacoes
GET  /api/professor/turmas/{turmaId}/avaliacoes
POST /api/professor/notas
GET  /api/professor/matriculas/{matriculaId}/notas
POST /api/professor/faltas
GET  /api/professor/matriculas/{matriculaId}/faltas
```

### Atividades e comunicação

```text
GET    /api/atividades
POST   /api/atividades
PUT    /api/atividades/{id}
DELETE /api/atividades/{id}
PUT    /api/atividades/{id}/entregas/{alunoId}

GET  /api/comunicados
POST /api/comunicados

GET  /api/eventos
POST /api/eventos
```

### Alunos e coordenação

```text
GET  /api/boletim/aluno/{id}
GET  /api/requerimentos
POST /api/requerimentos
PUT  /api/requerimentos/{id}
GET  /api/requerimentos/usuario/{id}
GET  /api/matriculas/cursos-disponiveis
POST /api/matriculas/solicitacoes
```

### Dashboards e serviços

```text
GET /api/dashboard/professor
GET /api/dashboard/coordenador
GET /api/audit/dashboard
GET /api/audit/logs
GET /api/salas
GET /api/configuracoes-portal
PUT /api/configuracoes-portal
GET /health
```

## Execução local

Pré-requisito:

- .NET SDK 10.

```powershell
dotnet restore
dotnet run --urls http://localhost:5055
```

Endereços:

```text
API:     http://localhost:5055
Swagger: http://localhost:5055/swagger
Health:  http://localhost:5055/health
```

Exemplo de health check:

```json
{
  "status": "healthy",
  "database": "Sqlite"
}
```

## Variáveis de ambiente

As configurações do `appsettings.json` podem ser substituídas usando `__`:

```powershell
$env:Database__Provider = "Sqlite"
$env:ConnectionStrings__DefaultConnection = "Data Source=data/educonnect.db"
$env:JwtSettings__SecretKey = "uma-chave-com-pelo-menos-32-caracteres"
$env:Cors__AllowedOrigins__0 = "http://localhost:5173"

dotnet run --urls http://localhost:5055
```

## Docker

```powershell
docker build -t educonnect-api .
docker run --rm -p 5055:5055 -v educonnect-data:/app/data educonnect-api
```

O volume `educonnect-data` preserva o SQLite entre reinícios.

Para produção, forneça credenciais e connection strings por secret manager ou arquivo de ambiente seguro.

## Build

```powershell
dotnet build
```

## Estado atual e próximos passos

A estrutura principal está funcional, mas alguns pontos devem evoluir antes de uma implantação institucional:

- automatizar auditoria para todas as operações críticas;
- substituir `EnsureCreated` por migrations em produção;
- implementar autorização por perfil em todos os endpoints legados;
- remover os dados mockados remanescentes do controller legado de desempenho acadêmico;
- implementar insights de curso calculados no backend;
- adicionar testes unitários e de integração;
- adicionar observabilidade centralizada com OpenTelemetry;
- armazenar logs operacionais em uma solução como CloudWatch, Seq ou Application Insights;
- utilizar URL pré-assinada ou CDN quando o bucket S3 não for público;
- rotacionar todos os segredos de produção.
