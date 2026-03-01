using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Modules.Autenticacao.Application.DTOs;
using Modules.Autenticacao.Application.UseCases;

namespace Modules.Autenticacao.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuarioUseCase _usuarioUseCase;

        public UsuariosController(UsuarioUseCase usuarioUseCase)
        {
            _usuarioUseCase = usuarioUseCase;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsuarios([FromQuery] PaginacaoFiltroDto filtro)
        {
            var response = await _usuarioUseCase.Execute(filtro);
            return Ok(response);
        }

        [HttpPost]
        public async Task<UsuarioRequestDto> PostUsuario(UsuarioRequestDto usuarioDto)
        {
            return await _usuarioUseCase.CriarUsuarioAsync(usuarioDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            await _usuarioUseCase.DeletarUsuarioAsync(id);
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsuario(int id)
        {
            var usuario = await _usuarioUseCase.ObterUsuarioPorIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return Ok(usuario);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, UsuarioDto usuarioDto)
        {
            await _usuarioUseCase.AtualizarUsuarioAsync(id, usuarioDto);
            return NoContent();
        }
    }
}
