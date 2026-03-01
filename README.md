# EduConnect - Backend

Sistema de educação com autenticação JWT, múltiplos módulos (Acadêmico, Financeiro, Biblioteca, Autenticação) e arquitetura em camadas (Clean Architecture).

## 🏗️ Estrutura do Projeto

```
/src
├── Api/
│   ├── Controllers/
│   ├── Middlewares/
│   └── Program.cs
├── Modules/
│   ├── Academico/
│   │   ├── Domain/
│   │   ├── Application/
│   │   ├── Infrastructure/
│   │   └── Api/
│   ├── Autenticacao/
│   │   ├── Domain/
│   │   ├── Application/
│   │   ├── Infrastructure/
│   │   └── Api/
│   ├── Financeiro/
│   ├── Biblioteca/
│   └── Shared/
└── Tests/
```

## 🔐 Autenticação

### Endpoint de Login

**POST** `/api/auth/login`

**Request:**
```json
{
  "email": "adm@edu.com",
  "senha": "123456"
}
```

**Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "usuario": {
    "id": 1,
    "nome": "Administrador do Sistema",
    "email": "adm@edu.com",
    "idPerfil": 1
  },
  "necessitaAceitarTermos": false
}
```

### Status Codes de Resposta

- `200 OK` - Login realizado com sucesso
- `400 Bad Request` - Email e/ou senha não fornecidos
- `401 Unauthorized` - Credenciais inválidas
- `403 Forbidden` - Usuário desativado
- `423 Locked` - Usuário bloqueado temporariamente (após 5 tentativas falhas)

## 🔑 Fluxo de Login

1. **Buscar usuário** pelo e-mail
2. **Validar status** (ativo)
3. **Verificar bloqueio** (tentativas e bloqueado_ate)
4. **Validar senha** com BCrypt
5. **Resetar tentativas** em caso de sucesso
6. **Atualizar último login**
7. **Gerar JWT** com claims (id, email, perfil)
8. **Validar aceite de termos** (se necessário)
9. **Retornar token e dados do usuário**

## 📋 Segurança

- ✅ Senhas hasheadas com BCrypt
- ✅ JWT com expiração configurável
- ✅ Limite de tentativas de login (5)
- ✅ Bloqueio temporário (30 minutos)
- ✅ Autenticação com claims
- ✅ Validação de token

## 🗄️ Banco de Dados

### Tabela: usuarios

| Campo | Tipo | Descrição |
|-------|------|-----------|
| id | INT | ID único |
| nome | NVARCHAR(150) | Nome completo |
| email | NVARCHAR(150) | Email único |
| senha_hash | NVARCHAR(255) | Hash da senha (BCrypt) |
| id_perfil | INT | Referência ao perfil |
| ativo | BIT | 1=ativo, 0=inativo |
| tentativas_falhas | INT | Contador de tentativas |
| bloqueado_ate | DATETIME | Data de desbloqueio |
| ultimo_login | DATETIME | Último login |
| data_aceite_termos | DATETIME | Data de aceite |
| versao_termos | INT | Versão aceita |
| data_criacao | DATETIME | Data de criação |
| data_atualizacao | DATETIME | Data de atualização |

## 🚀 Como Executar

### Pré-requisitos
- .NET 7.0+
- SQL Server
- Visual Studio ou VS Code

### Passos

1. **Clonar o repositório**
```bash
git clone <repo-url>
cd back-educonnect
```

2. **Restaurar dependências**
```bash
dotnet restore
```

3. **Criar banco de dados**
```bash
# Execute o script DATABASE_SCHEMA.sql no SQL Server
```

4. **Configurar appsettings.json**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=SEU_SERVER;Database=back_educonnect;User Id=sa;Password=SUA_SENHA;"
  },
  "JwtSettings": {
    "SecretKey": "sua_chave_secreta_segura_com_minimo_32_caracteres",
    "Issuer": "EduConnect",
    "Audience": "EduConnectUsers",
    "ExpirationMinutes": 60
  }
}
```

5. **Executar**
```bash
dotnet run
```

6. **Acessar Swagger**
```
https://localhost:5001/swagger
```

## 🧪 Testando o Login

### Com Curl
```bash
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"adm@edu.com","senha":"123456"}'
```

### Com Postman
1. Novo POST request
2. URL: `https://localhost:5001/api/auth/login`
3. Body (raw JSON):
```json
{
  "email": "adm@edu.com",
  "senha": "123456"
}
```

## 🔑 Usando o Token

Adicione o token no header de requisições autenticadas:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## 📦 Dependências

- `Microsoft.AspNetCore.Authentication.JwtBearer` - Autenticação JWT
- `System.IdentityModel.Tokens.Jwt` - Manipulação de JWT
- `BCrypt.Net-Core` - Hash de senhas
- `System.Data.SqlClient` - Acesso ao SQL Server

## 📝 Arquitetura

### Clean Architecture com DDD

- **Domain**: Entidades, Interfaces, Regras de negócio
- **Application**: UseCases, DTOs, Commands/Queries
- **Infrastructure**: Repositories, Services, Banco de dados
- **Api**: Controllers, Middlewares

## 🤝 Contribuindo

1. Fork o projeto
2. Crie uma branch (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença MIT.
