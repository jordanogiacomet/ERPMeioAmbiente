using ERPMeioAmbiente.Shared;
using ERPMeioAmbienteAPI.Data.Dtos;
using ERPMeioAmbienteAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace ERPMeioAmbienteAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        [SwaggerOperation(Summary = "Registrar um novo usuário", Description = "Registra um novo usuário e cria um registro de cliente associado")]
        [SwaggerResponse(200, "Usuário registrado com sucesso", typeof(UserManegerResponse))]
        [SwaggerResponse(400, "Falha no registro do usuário", typeof(UserManegerResponse))]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterViewModel model)
        {
            var result = await _userService.RegisterUserAsync(model);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPost("login")]
        [SwaggerOperation(Summary = "Login de um usuário", Description = "Autentica um usuário e retorna um token JWT")]
        [SwaggerResponse(200, "Usuário autenticado com sucesso", typeof(UserManegerResponse))]
        [SwaggerResponse(400, "Falha na autenticação do usuário", typeof(UserManegerResponse))]
        public async Task<IActionResult> LoginAsync([FromBody] LoginViewModel model)
        {
            var result = await _userService.LoginUserAsync(model);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPost("forgot-password")]
        [SwaggerOperation(Summary = "Esqueci minha senha", Description = "Inicia o processo de recuperação de senha")]
        [SwaggerResponse(200, "Link de recuperação enviado", typeof(UserManegerResponse))]
        [SwaggerResponse(400, "Falha ao iniciar o processo de recuperação de senha", typeof(UserManegerResponse))]
        public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordViewModel model)
        {
            var result = await _userService.ForgotPasswordAsync(model);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPost("reset-password")]
        [SwaggerOperation(Summary = "Redefinir senha", Description = "Redefine a senha do usuário")]
        [SwaggerResponse(200, "Senha redefinida com sucesso", typeof(UserManegerResponse))]
        [SwaggerResponse(400, "Falha ao redefinir a senha", typeof(UserManegerResponse))]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordViewModel model)
        {
            var result = await _userService.ResetPasswordAsync(model);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPost("confirm-email")]
        [SwaggerOperation(Summary = "Confirmar e-mail", Description = "Confirma o e-mail do usuário")]
        [SwaggerResponse(200, "E-mail confirmado com sucesso", typeof(UserManegerResponse))]
        [SwaggerResponse(400, "Falha ao confirmar o e-mail", typeof(UserManegerResponse))]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var result = await _userService.ConfirmEmailAsync(userId, token);

            if (result.IsSuccess)
            {
                return Ok(result.Message); // Ou redirecionar para uma página de sucesso
            }

            return BadRequest(result); // Ou redirecionar para uma página de erro
        }
    }
}
