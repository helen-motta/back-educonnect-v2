# Autenticação com Cookies - Persistência de 2 Horas

## Visão Geral

Implementação de autenticação persistente por cookies com expiração de 2 horas, seguindo os princípios da **Clean Architecture**.

## Arquitetura

### Domain Layer (Interfaces)
- **ICookieService**: Interface que define o contrato para gerenciamento de cookies
  - `SetAuthenticationCookie(HttpResponse response, string token, int expirationHours)`: Define o cookie de autenticação
  - `RemoveAuthenticationCookie(HttpResponse response)`: Remove o cookie de autenticação

### Infrastructure Layer (Implementação)
- **CookieService**: Implementação conreta do `ICookieService`
  - Encapsula toda a lógica de cookie (segurança, expiração)
  - Utiliza `HttpOnly = true` (previne XSS)
  - Utiliza `Secure = true` (apenas HTTPS)
  - Utiliza `SameSite = Strict` (previne CSRF)

### Application Layer (Use Cases)
- **LoginUseCase**: Não foi modificado (responsabilidade única mantida)
- Permanece gerando o token JWT normalmente

### API Layer (Controllers)
- **AuthController**: 
  - Injeta `ICookieService` por DI
  - **Login**: Após autenticação bem-sucedida, seta o cookie com 2h de expiração
  - **Logout**: Remove o cookie (novo endpoint)

## Fluxo de Autenticação

```
1. Client envia POST /api/auth/login com email/senha
2. LoginUseCase valida e gera JWT token
3. AuthController recebe o token
4. AuthController chama _cookieService.SetAuthenticationCookie()
5. Servidor responde com Set-Cookie header
6. Browser automaticamente envia cookie em próximas requisições
7. Program.cs (OnMessageReceived) extrai o token do cookie
8. JWT é validado normalmente
```

## Fluxo de Logout

```
1. Client envia POST /api/auth/logout
2. AuthController chama _cookieService.RemoveAuthenticationCookie()
3. Servidor responde com Set-Cookie header (Delete)
4. Browser remove o cookie
```

## Configuração do Program.cs

### 1. Registro de Dependências
```csharp
builder.Services.AddScoped<ICookieService, CookieService>();
```

### 2. Configuração JWT + Cookies
```csharp
options.Events = new JwtBearerEvents
{
    OnMessageReceived = context =>
    {
        context.Token = context.Request.Cookies["X-Access-Token"];
        return Task.CompletedTask;
    }
};
```

### 3. CORS com Credentials
```csharp
.AllowCredentials(); // Essencial para enviar cookies em requests CORS
```

## Segurança

### Cookies
- ✅ **HttpOnly**: Não acessível via JavaScript (protege contra XSS)
- ✅ **Secure**: Apenas enviado via HTTPS (protege contra man-in-the-middle)
- ✅ **SameSite=Strict**: Protege contra CSRF

### Token JWT
- ✅ Continua sendo validado no servidor
- ✅ Expiração de 2 horas (configurável em `JwtSettings:ExpirationMinutes`)
- ✅ Assinado com chave secreta

## Endpoints

### Login
```
POST /api/auth/login
Content-Type: application/json

{
  "email": "usuario@example.com",
  "senha": "senha123"
}

Response: 200 OK
Set-Cookie: X-Access-Token={jwt_token}; HttpOnly; Secure; SameSite=Strict; Expires=...
```

### Logout
```
POST /api/auth/logout

Response: 200 OK
Set-Cookie: X-Access-Token=; Expires=Thu, 01 Jan 1970 00:00:00 GMT
```

## Frontend (Cliente)

### Requisições com Cookie
```javascript
// O navegador automaticamente envia o cookie
fetch('http://localhost:5000/api/dados', {
  credentials: 'include', // IMPORTANTE: inclui cookies
  headers: {
    'Content-Type': 'application/json'
  }
})
```

### Logout
```javascript
fetch('http://localhost:5000/api/auth/logout', {
  method: 'POST',
  credentials: 'include'
})
```

## Timeouts e Renovação

A sessão expira em **2 horas**. Opções:

1. **Renovação automática**: Client faz refresh da página/toca um endpoint antes de expirar
2. **Refresh Token**: Implementar endpoint que gera novo token sem fazer login novamente
3. **SPA com verificação**: Verificar expiração do JWT no cliente

## Variáveis de Ambiente (appsettings.json)

```json
{
  "JwtSettings": {
    "SecretKey": "sua-chave-secreta-super-segura",
    "Issuer": "sua-app",
    "Audience": "sua-app-users",
    "ExpirationMinutes": 120
  }
}
```

## Próximos Passos (Opcional)

1. **Refresh Token**: Adicionar endpoint para renovar sessão sem fazer login
2. **Redis**: Armazenar sessões em cache para controle adicional
3. **Auditoria**: Registrar logins/logouts por IP e device
4. **2FA**: Adicionar autenticação de dois fatores

---

**Arquitetura**: Clean Architecture (Domain → Infrastructure)
**Padrão**: Dependency Injection + Interface Segregation
**Segurança**: HTTPS + HttpOnly + SameSite + JWT assinado
