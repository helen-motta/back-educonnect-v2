using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Modules.Autenticacao.Application.DTOs;
using Modules.Autenticacao.Application.UseCases;

namespace Modules.Autenticacao.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PerfilController : ControllerBase
    {
        private readonly UsuarioUseCase _perfilUseCase;

        public PerfilController(UsuarioUseCase perfilUseCase)
        {
            _perfilUseCase = perfilUseCase;
        }

        [HttpGet]
        public async Task<IActionResult> GetPerfil()
        {
            var userIdClaim = User.FindFirst("sub")?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Usuário não autenticado.");
            }

            var response = await _perfilUseCase.ObterUsuarioPorIdAsync(userId);
            if (response == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            return Ok(response);
        }
    }
}
