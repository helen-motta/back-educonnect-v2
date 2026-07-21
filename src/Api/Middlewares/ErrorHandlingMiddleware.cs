using System.Net;

namespace Api.Middlewares;

public sealed class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger) { _next = next; _logger = logger; }

    public async Task InvokeAsync(HttpContext context)
    {
        try { await _next(context); }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Erro ao processar {Method} {Path}", context.Request.Method, context.Request.Path);
            var status = exception switch
            {
                ArgumentException => HttpStatusCode.BadRequest,
                UnauthorizedAccessException => HttpStatusCode.Forbidden,
                KeyNotFoundException => HttpStatusCode.NotFound,
                _ => HttpStatusCode.InternalServerError
            };
            context.Response.StatusCode = (int)status;
            await context.Response.WriteAsJsonAsync(new { message = status == HttpStatusCode.InternalServerError ? "Ocorreu um erro interno no servidor." : exception.Message });
        }
    }
}
