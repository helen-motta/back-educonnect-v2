# Integração SendGrid - Guia de Configuração

## 📧 Overview

A integração SendGrid foi adicionada ao projeto para envio seguro de e-mails de reset de senha. O fluxo segue as melhores práticas de segurança:
- **Token gerado**: 256 bits (32 bytes), codificado em base64url
- **Expiração**: 15 minutos
- **Comparação segura**: `CryptographicOperations.FixedTimeEquals` (previne timing attacks)
- **Privacidade**: Endpoint de solicitação não revela existência de conta

---

## 🔧 Passos de Configuração

### 1. Configurar Chave API no SendGrid

1. Acesse [SendGrid Dashboard](https://app.sendgrid.com)
2. Navegue até **Settings** → **API Keys**
3. Clique em **Create API Key**
4. Nomeie como `EduConnect-API` (ou similar)
5. Copie a chave gerada (formato: `SG.xxxxx`)

### 2. Configurar Domínio no SendGrid

Para validar que os e-mails vêm de seu domínio e melhorar taxa de entrega:

1. No SendGrid Dashboard, vá para **Settings** → **Sender Authentication**
2. Clique em **Authenticate Your Domain**
3. Escolha domínio `edu.br`
4. Adicione os registros CNAME fornecidos (já temos):

```
Type    Host                         Value
-----   ----                         -----
CNAME   em9110.edu.br                u58214971.wl222.sendgrid.net
CNAME   s1._domainkey.edu.br         s1.domainkey.u58214971.wl222.sendgrid.net
CNAME   s2._domainkey.edu.br         s2.domainkey.u58214971.wl222.sendgrid.net
TXT     _dmarc.edu.br                v=DMARC1; p=none;
```

5. Aguarde validação (alguns minutos até 48h)

### 3. Configurar `appsettings.json`

Substitua os valores no arquivo:

```json
"SendGrid": {
  "ApiKey": "SG.seu_sendgrid_api_key_aqui",
  "FromEmail": "noreply@edu.br",
  "FromName": "EduConnect"
}
```

⚠️ **Segurança em Produção**:
- Nunca commit a chave API no repositório
- Use **User Secrets** localmente (desenvolvimento):
  ```powershell
  dotnet user-secrets init
  dotnet user-secrets set "SendGrid:ApiKey" "SG.xxxxx"
  ```
- Em produção, use **Azure Key Vault**, **AWS Secrets Manager** ou variáveis de ambiente

### 4. Adicionar Sender Email no SendGrid

1. No SendGrid, vá para **Settings** → **Sender Verification**
2. Clique em **Verify a Single Sender**
3. Use o e-mail configurado (`noreply@edu.br`)
4. Confirme o e-mail de verificação

---

## 📋 Arquivos Alterados/Criados

### Criados
- `src/Modules/Autenticacao/Domain/Interfaces/IEmailService.cs` — Interface de serviço de e-mail
- `src/Modules/Autenticacao/Infrastructure/Services/SendGridEmailService.cs` — Implementação com SendGrid

### Modificados
- `back-educonnect.csproj` — Adicionado pacote `SendGrid` v9.28.1
- `src/Api/Program.cs` — Injeção de dependência de `IEmailService`
- `src/Modules/Autenticacao/Application/UseCases/LoginUseCase.cs` — Integração de envio de e-mail no `SolicitarResetAsync`
- `appsettings.json` — Adicionada seção `SendGrid`

---

## 🔌 API Endpoints

### 1. Solicitar Reset de Senha
**POST** `/api/Auth/esqueci-senha`

**Request Body**:
```json
{
  "email": "usuario@exemplo.com"
}
```

**Response** (sempre 200 por segurança):
```json
{
  "message": "Se o e-mail existir, você receberá instruções para redefinir a senha."
}
```

**O que acontece internamente**:
- ✅ Valida se o usuário existe
- ✅ Gera token seguro (256 bits)
- ✅ Define expiração (15 minutos)
- ✅ Salva no banco de dados
- ✅ **Envia e-mail com link de reset** (via SendGrid)

---

### 2. Efetuar Reset de Senha
**POST** `/api/Auth/reset-senha`

**Request Body**:
```json
{
  "email": "usuario@exemplo.com",
  "token": "AZb4X9pQ...",
  "novaSenha": "NovaSenh@Segura123"
}
```

**Resposta de Sucesso** (200 OK):
```json
{
  "message": "Senha atualizada com sucesso"
}
```

**Respostas de Erro**:
```json
// 400 Bad Request
{
  "message": "Token expirado"
}

// 400 Bad Request
{
  "message": "Token inválido"
}

// 400 Bad Request
{
  "message": "Usuário não encontrado"
}
```

---

## 📧 Template de E-mail

O e-mail enviado inclui:
- ✅ Link de reset (adaptável para seu frontend)
- ✅ Aviso de segurança (expiração 15 min)
- ✅ Proteção contra phishing
- ✅ Design responsivo (desktop/mobile)
- ✅ Branding EduConnect

**Nota**: Atualize a URL do link no arquivo `LoginUseCase.cs`:
```csharp
var resetLink = $"https://seu-frontend.com.br/reset-senha?email={Uri.EscapeDataString(usuario.Email)}&token={Uri.EscapeDataString(token)}";
```

---

## 🔐 Segurança

### Token Generation
```csharp
var tokenBytes = new byte[32];
RandomNumberGenerator.Fill(tokenBytes);  // Criptograficamente seguro
var token = WebEncoders.Base64UrlEncode(tokenBytes);
```

### Comparação Segura (Timing Attack Prevention)
```csharp
if (!CryptographicOperations.FixedTimeEquals(provided, stored))
    throw new InvalidOperationException("Token inválido");
```

### Hashing de Senha
```csharp
usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.NovaSenha);
```

### Email Não Revela Existência
```csharp
// Endpoint retorna 200 OK mesmo se usuário não existe
if (usuario is null)
    return;  // Sem revelar ao cliente
```

---

## 🧪 Testando

### Via Postman/cURL

**1. Solicitar token:**
```bash
curl -X POST https://localhost:7001/api/Auth/esqueci-senha \
  -H "Content-Type: application/json" \
  -d '{"email": "usuario@exemplo.com"}'
```

**2. Check e-mail** (SendGrid deve ter enviado)

**3. Usar token para reset:**
```bash
curl -X POST https://localhost:7001/api/Auth/reset-senha \
  -H "Content-Type: application/json" \
  -d '{
    "email": "usuario@exemplo.com",
    "token": "token_recebido_no_email",
    "novaSenha": "NovaSenha@123"
  }'
```

### Monitorar SendGrid
- SendGrid Dashboard → **Mail Activity** (logs de envio)
- **Bounce Management** (e-mails inválidos)
- **Spam Reports** (queixas)

---

## ⚠️ Troubleshooting

| Problema | Solução |
|----------|---------|
| "SendGrid API key is not configured" | Verify `appsettings.json` tem a chave `SendGrid:ApiKey` |
| E-mail não chega | 1. Verificar log SendGrid Dashboard<br>2. Validar domínio (CNAME/DNS)<br>3. Check spam folder |
| "FromEmail is not configured" | Adicione `SendGrid:FromEmail` no `appsettings.json` |
| Token "expirado" quando é novo | Verificar timezone do servidor (use `DateTime.UtcNow`) |
| Erro ao conectar SendGrid | Verificar internet, firewall, firewall de saída (port 587) |

---

## 📚 Referências

- [SendGrid Official Docs](https://docs.sendgrid.com/)
- [SendGrid C# SDK](https://github.com/sendgrid/sendgrid-csharp)
- [OWASP - Password Reset Token](https://cheatsheetseries.owasp.org/cheatsheets/Forgot_Password_Cheat_Sheet.html)
- [RFC 2104 - HMAC](https://tools.ietf.org/html/rfc2104)

---

## ✅ Checklist de Deploy em Produção

- [ ] API Key configurada em variáveis de ambiente (não no código)
- [ ] Domínio validado no SendGrid (CNAME/DNS propagados)
- [ ] Sender email verificado no SendGrid
- [ ] URL de reset-senha aponta para frontend correto
- [ ] Testar fluxo completo de reset
- [ ] Monitorar taxa de bounce/spam
- [ ] Implementar logging/alertas para falhas de e-mail
- [ ] HTTPS ativado em produção
- [ ] Rate limiting no endpoint (prevent abuse)
- [ ] Documentação para suporte
