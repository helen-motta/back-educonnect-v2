using System.Text;
using System.Text.Json.Serialization;
using Api.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Modules.Academico.Application.UseCases;
using Modules.Academico.Domain.Interfaces;
using Modules.Academico.Infrastructure.Persistence.Repositories;
using Modules.Autenticacao.Application.UseCases;
using Modules.Autenticacao.Domain.Interfaces;
using Modules.Autenticacao.Infrastructure.Persistence.Repositories;
using Modules.Autenticacao.Infrastructure.Services;
using Shared.Domain.Interfaces;
using Shared.Infrastructure;
using Shared.Infrastructure.Storage;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

var databaseProvider = builder.Configuration["Database:Provider"] ?? "Sqlite";
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection não foi configurada.");

if (databaseProvider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
{
    Directory.CreateDirectory(Path.Combine(builder.Environment.ContentRootPath, "data"));
    builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));
}
else if (databaseProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
}
else
{
    throw new InvalidOperationException($"Provedor de banco '{databaseProvider}' não suportado.");
}

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IAlunoRepository, AlunoRepository>();
builder.Services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();
builder.Services.AddScoped<ICursosRepository, CursosRepository>();
builder.Services.AddScoped<IDisciplinasRepository, DisciplinasRepository>();
builder.Services.AddScoped<ITurmasRepository, TurmasRepository>();
builder.Services.AddScoped<IFrequenciaRepository, FrequenciaRepository>();
builder.Services.AddScoped<IRequerimentosRepository, RequerimentosRepository>();
builder.Services.AddScoped<IDocumentoRepository, DocumentoRepository>();
builder.Services.AddScoped<IEventoRepository, EventoRepository>();
builder.Services.AddScoped<IProfessorLancamentosRepository, ProfessorLancamentosRepository>();
builder.Services.AddScoped<IPortalDataRepository, PortalDataRepository>();

builder.Services.AddScoped<LoginUseCase>();
builder.Services.AddScoped<UsuarioUseCase>();
builder.Services.AddScoped<CursosUseCase>();
builder.Services.AddScoped<TurmasUseCase>();
builder.Services.AddScoped<DisciplinasUseCase>();
builder.Services.AddScoped<AuditoriaUseCase>();
builder.Services.AddScoped<RequerimentosUseCase>();
builder.Services.AddScoped<DocumentoUseCase>();
builder.Services.AddScoped<ListarEventosUseCase>();
builder.Services.AddScoped<ProfessorLancamentosUseCase>();
builder.Services.AddScoped<PortalUseCase>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.Configure<S3Options>(builder.Configuration.GetSection(S3Options.SectionName));
builder.Services.AddScoped<IFileStorageService, S3FileStorageService>();
if (builder.Configuration["Email:Provider"]?.Equals("SendGrid", StringComparison.OrdinalIgnoreCase) == true)
    builder.Services.AddScoped<IEmailService, SendGridEmailService>();
else
    builder.Services.AddScoped<IEmailService, DevelopmentEmailService>();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];
if (string.IsNullOrWhiteSpace(secretKey) || secretKey.Length < 32)
    throw new InvalidOperationException("JwtSettings:SecretKey deve ter ao menos 32 caracteres.");

System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.MapInboundClaims = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.TryGetValue("X-Access-Token", out var token))
                context.Token = token;
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "EduConnect API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        [new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
        }] = Array.Empty<string>()
    });
});

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:5173"];
builder.Services.AddCors(options => options.AddPolicy("Frontend", policy =>
    policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

var app = builder.Build();
await DatabaseInitializer.InitializeAsync(app.Services, app.Configuration);

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseCors("Frontend");
app.UseSwagger();
app.UseSwaggerUI();
if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "healthy", database = databaseProvider }));
app.Run();

public partial class Program;
