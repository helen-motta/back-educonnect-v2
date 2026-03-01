using Microsoft.AspNetCore.Mvc;
using Modules.Autenticacao.Application.DTOs;
using Modules.Autenticacao.Application.UseCases;

namespace Modules.Autenticacao.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly LoginUseCase _loginUseCase;

        public AuthController(LoginUseCase loginUseCase)
        {
            _loginUseCase = loginUseCase;
        }

        /// <summary>
        /// Realiza login do usuário
        /// </summary>
        /// <param name="request">Email e senha do usuário</param>
        /// <returns>Token JWT e informações do usuário</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Dados inválidos" });
            }

            var (response, statusCode) = await _loginUseCase.ExecutarAsync(request);

            if ((int)statusCode == StatusCodes.Status401Unauthorized)
            {
                return Unauthorized(new { message = "Credenciais inválidas" });
            }

            if ((int)statusCode == 403)
            {
                return StatusCode(403, new { message = "Usuário desativado;" });
            }

            if ((int)statusCode == 423)
            {
                return StatusCode(423, new { message = "Usuário bloqueado temporariamente. Tente novamente mais tarde." });
            }

            if ((int)statusCode == StatusCodes.Status400BadRequest)
            {
                return BadRequest(new { message = "Email e senha são obrigatórios" });
            }

            if (response == null)
            {
                return StatusCode((int)statusCode, new { message = "Erro ao realizar login" });
            }
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(2) 
            };

            Response.Cookies.Append("X-Access-Token", response.Token, cookieOptions);
        
            return Ok(response);
        }

        /// <summary>
        /// Solicita geração de token de reset (não revela existência de conta)
        /// </summary>
        [HttpPost("esqueci-senha")]
        public async Task<IActionResult> SolicitarReset([FromBody] EsqueceuSenhaDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Dados inválidos" });

            var token = await _loginUseCase.SolicitarResetAsync(request);

            return Ok(new 
            { 
                mensagem = "Se o e-mail existir, você receberá instruções para redefinir a senha.",
                token = token
            });
        }

        /// <summary>
        /// Efetiva a troca de senha usando email + token + nova senha
        /// </summary>
        [HttpPost("reset-senha")]
        public async Task<IActionResult> ResetarSenha([FromBody] ResetSenhaDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Dados inválidos" });

            try
            {
                await _loginUseCase.EfetuarResetAsync(request);
                return Ok(new { message = "Senha atualizada com sucesso" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch
            {
                return StatusCode(500, new { message = "Erro interno" });
            }
        }
    }
}
