using ERPMeioAmbiente.Shared;
using ERPMeioAmbienteAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace ERPMeioAmbienteAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Register")]
        [SwaggerOperation(Summary = "Registra um novo usuário", Description = "Registra um novo usuário no sistema")]
        [SwaggerResponse(200, "Usuário registrado com sucesso")]
        [SwaggerResponse(400, "Propriedades inválidas")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.RegisterUserAsync(model);

                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }

            return BadRequest("Properties not valid");
        }

        [HttpPost("Login")]
        [SwaggerOperation(Summary = "Login do usuário", Description = "Realiza o login do usuário no sistema")]
        [SwaggerResponse(200, "Login realizado com sucesso")]
        [SwaggerResponse(400, "Propriedades inválidas")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.LoginUserAsync(model);

                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Some properties are not valid");
        }

        [HttpPost("ForgotPassword")]
        [SwaggerOperation(Summary = "Recuperar senha", Description = "Envia um link de recuperação de senha para o email do usuário")]
        [SwaggerResponse(200, "Link de recuperação enviado com sucesso")]
        [SwaggerResponse(400, "Propriedades inválidas")]
        public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.ForgotPasswordAsync(model);

                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }

            return BadRequest("Properties not valid");
        }

        [HttpPost("ResetPassword")]
        [SwaggerOperation(Summary = "Redefinir senha", Description = "Redefine a senha do usuário utilizando o token de recuperação")]
        [SwaggerResponse(200, "Senha redefinida com sucesso")]
        [SwaggerResponse(400, "Propriedades inválidas")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.ResetPasswordAsync(model);

                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }

            return BadRequest("Properties not valid");
        }
    }
}
