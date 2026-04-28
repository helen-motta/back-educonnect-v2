using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Shared.Api
{
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        protected string ObterUsuarioId()
        {
            return User.FindFirst("sub")?.Value ?? "anonimo";
        }

        protected string? ObterEnderecoIp()
        {
            return HttpContext.Connection.RemoteIpAddress?.ToString();
        }

        protected string? ObterUserAgent()
        {
            return Request.Headers.UserAgent.ToString();
        }
    }
}
