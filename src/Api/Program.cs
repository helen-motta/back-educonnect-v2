using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Modules.Autenticacao.Domain.Interfaces;
using Modules.Autenticacao.Infrastructure.Persistence.Repositories;
using Modules.Autenticacao.Infrastructure.Services;
using Modules.Autenticacao.Application.UseCases;
using Modules.Academico.Domain.Interfaces;
// using Modules.Academico.Domain.Services;
// using Modules.Academico.Application.UseCases;
using Modules.Academico.Infrastructure.Persistence.Repositories;
using Shared.Infrastructure;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;
using DesempenhoAcademicoMock;
using Modules.Academico.Application.UseCases;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEmailService, SendGridEmailService>();
builder.Services.AddScoped<ICursosRepository, CursosRepository>();
builder.Services.AddScoped<ITurmasRepository, TurmasRepository>();
builder.Services.AddScoped<LoginUseCase>();
builder.Services.AddScoped<UsuarioUseCase>(); 
builder.Services.AddScoped<CursosUseCase>(); 
builder.Services.AddScoped<TurmasUseCase>();
builder.Services.AddScoped<DisciplinasUseCase>(); 
builder.Services.AddScoped<RequerimentosUseCase>();
builder.Services.AddScoped<DocumentoUseCase>();
builder.Services.AddScoped<ListarEventosUseCase>();

// Registrar dependências do módulo Acadêmico
builder.Services.AddScoped<IAlunoRepository, AlunoRepository>();
builder.Services.AddScoped<IDisciplinasRepository, DisciplinasRepository>();
// builder.Services.AddScoped<IAvaliacaoRepository, AvaliacaoRepository>();
builder.Services.AddScoped<IFrequenciaRepository, FrequenciaRepository>();
// builder.Services.AddScoped<DesempenhoAcademicoService>();
// builder.Services.AddScoped<CalcularDesempenhoAcademicoUseCase>();
builder.Services.AddScoped<IRequerimentosRepository, RequerimentosRepository>();
builder.Services.AddScoped<IDocumentoRepository, DocumentoRepository>();
builder.Services.AddScoped<IEventoRepository, EventoRepository>();


var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];
System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters 
        { 
             ValidateIssuerSigningKey = true,
             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
             ValidateIssuer = true,
             ValidIssuer = issuer,
             ValidateAudience = true,
             ValidAudience = audience,
             ValidateLifetime = true,
             ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Verifica se tem token no Cookie. 
                // Se NÃO tiver, ele deixa o .NET procurar no cabeçalho Authorization (padrão)
                var cookieToken = context.Request.Cookies["X-Access-Token"];
                if (!string.IsNullOrEmpty(cookieToken))
                {
                    context.Token = cookieToken;
                }
                return Task.CompletedTask;
            }
        };
    });
    
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API de Autenticação",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Informe o token JWT no formato: Bearer {seu_token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("MinhaPoliticaCors", policy =>
    {
        policy.WithOrigins("http://localhost:5173") 
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
var app = builder.Build();

app.UseCors("MinhaPoliticaCors");

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();