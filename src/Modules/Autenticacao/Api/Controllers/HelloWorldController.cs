using Microsoft.AspNetCore.Mvc;

namespace Modules.Autenticacao.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HelloWorldController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("hello, World!");
        }
    }
}
