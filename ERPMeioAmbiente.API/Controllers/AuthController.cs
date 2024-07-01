using ERPMeioAmbiente.Shared;
using ERPMeioAmbienteAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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

    [HttpGet("ConfirmEmail")]
    [SwaggerOperation(Summary = "Confirma o email do usuário", Description = "Confirma o email do usuário no sistema")]
    [SwaggerResponse(200, "Email confirmado com sucesso")]
    [SwaggerResponse(400, "Token de confirmação inválido")]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        var result = await _userService.ConfirmEmailAsync(userId, token);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }
}
