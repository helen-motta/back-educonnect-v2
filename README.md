# EduConnect API

Backend do EduConnect em ASP.NET Core, organizado por módulos e preparado para iniciar sem que o desenvolvedor precise obter uma cópia do banco de dados.

## Execução rápida

Pré-requisitos: [.NET SDK 10](https://dotnet.microsoft.com/download) e, para o frontend, Node.js 20 ou superior.

```powershell
dotnet restore
dotnet run --urls http://localhost:5055
```

Na primeira execução, a aplicação cria automaticamente `data/educonnect.db` (SQLite), monta o esquema e inclui dados de demonstração. A documentação interativa fica em `http://localhost:5055/swagger` e o estado do serviço em `http://localhost:5055/health`.

Não é preciso clonar, restaurar ou executar scripts de banco para desenvolvimento local. Para recriar a base, encerre a API e apague `data/educonnect.db`; ela será gerada novamente no próximo início.

## Contas de demonstração

Todas usam a senha `123456`.

| Perfil | E-mail |
| --- | --- |
| Administrador | `admin@educonnect.local` |
| Coordenador | `coordenador@educonnect.local` |
| Professor | `professor@educonnect.local` |
| Aluno | `aluno@educonnect.local` |

Essas contas existem apenas quando `Database__SeedDemoData=true`.

## Configuração

Os valores locais estão em `appsettings.json`. Toda opção pode ser substituída por variável de ambiente usando `__` entre níveis:

```powershell
$env:Database__Provider = "Sqlite"
$env:ConnectionStrings__DefaultConnection = "Data Source=data/educonnect.db"
$env:JwtSettings__SecretKey = "uma-chave-com-pelo-menos-32-caracteres"
dotnet run --urls http://localhost:5055
```

### SQL Server em produção

O arquivo `appsettings.Production.example.json` contém um modelo. Copie-o para `appsettings.Production.json`, substitua os valores e execute com `ASPNETCORE_ENVIRONMENT=Production`. Nesse modo, use `Database__Provider=SqlServer` e uma conexão SQL Server válida. `Initialize=false` evita alterações automáticas no banco de produção.

### Imagens no Amazon S3

Fotos de perfil são validadas como imagem (JPEG, PNG, WebP ou GIF, até 5 MB) e enviadas ao bucket S3 pela rota `POST /api/perfil/foto`. As chaves seguem `usuarios/AAAA/MM/<guid>.<extensão>`.

As credenciais presentes em `appsettings.json` são deliberadamente fictícias. Configure valores reais por secret manager ou variáveis de ambiente:

```text
AWS__Region=sa-east-1
AWS__BucketName=educonnect-imagens-prod
AWS__AccessKey=<access-key>
AWS__SecretKey=<secret-key>
AWS__PublicBaseUrl=https://cdn.exemplo.com
```

O usuário IAM precisa de `s3:PutObject` no prefixo `usuarios/*`. Não versione credenciais reais. O bucket ou CDN deve permitir leitura das imagens retornadas ao frontend.

### E-mail local

Em desenvolvimento, `Email__Provider=Console` registra os e-mails de recuperação no terminal e elimina a dependência de um provedor externo.
Em produção, use `Email__Provider=SendGrid` e configure `SendGrid__ApiKey`, `SendGrid__FromEmail` e `SendGrid__FromName`.

## Arquitetura

```text
src/
├── Api/                     inicialização, banco local, seed e middleware
├── Modules/
│   ├── Autenticacao/
│   │   ├── Api/             controllers
│   │   ├── Application/     DTOs e casos de uso
│   │   ├── Domain/          entidades e contratos
│   │   └── Infrastructure/  persistência e serviços
│   └── Academico/           mesma divisão por camadas
└── Shared/                  contexto EF, storage S3 e contratos comuns
```

Controllers cuidam de HTTP, casos de uso concentram regras, repositórios encapsulam persistência e entidades representam o domínio. As funcionalidades antes estáticas — dashboards, comunicados, atividades, salas, matrícula, configurações e turmas — usam o mesmo fluxo e persistem no banco.

## Rotas principais

- `POST /api/auth/login`: autenticação JWT.
- `GET /api/perfil`: usuário autenticado.
- `GET /api/dashboard/{perfil}`: painéis de professor e coordenador.
- `GET /api/turmas`, `/api/atividades`, `/api/comunicados`: domínio acadêmico.
- `GET /api/matriculas/cursos-disponiveis`: catálogo sem dados fixos no frontend.
- `GET /api/audit/dashboard`: painel administrativo.

Use `Authorization: Bearer <token>` nas rotas protegidas.

## Docker

```powershell
docker build -t educonnect-api .
docker run --rm -p 5055:5055 -v educonnect-data:/app/data educonnect-api
```

O volume preserva o SQLite entre reinícios. Para produção, forneça as configurações por `--env-file` ou secret manager.

## Verificação

```powershell
dotnet build
```

O projeto ainda contém avisos de nulabilidade em modelos legados, mas deve compilar sem erros. Para uma checagem funcional, inicie a API, abra `/health` e autentique uma das contas de demonstração.
