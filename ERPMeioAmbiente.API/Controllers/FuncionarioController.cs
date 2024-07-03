using AutoMapper;
using ERPMeioAmbiente.API.Data.Dtos;
using ERPMeioAmbienteAPI.Data.Dtos;
using ERPMeioAmbienteAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPMeioAmbiente.API.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize] // Proteger todas as rotas
    public class FuncionarioController : ControllerBase
    {
        private readonly IFuncionarioService _funcionarioService;
        private readonly IMapper _mapper;

        public FuncionarioController(IFuncionarioService funcionarioService, IMapper mapper)
        {
            _funcionarioService = funcionarioService;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize]
        [SwaggerOperation(Summary = "Adiciona um novo funcionário", Description = "Adiciona um novo funcionário ao sistema")]
        [SwaggerResponse(201, "Funcionário criado com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        public async Task<IActionResult> AdicionaFuncionario([FromBody] CreateFuncionarioDto funcionarioDto)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }

            try
            {
                var funcionario = await _funcionarioService.AddFuncionarioAsync(funcionarioDto);
                return CreatedAtAction(nameof(RecuperaFuncionarioPorId), new { id = funcionario.Id }, funcionario);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = "Erro ao criar funcionário", errors = ex.Message });
            }
        }

        [HttpGet]
        [Authorize]
        [SwaggerOperation(Summary = "Recupera todos os funcionários", Description = "Recupera uma lista de todos os funcionários do sistema")]
        [SwaggerResponse(200, "Lista de funcionários recuperada com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        public async Task<IActionResult> RecuperaFuncionarios([FromQuery] int skip = 0, [FromQuery] int take = 50)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }
            var funcionarios = await _funcionarioService.GetAllFuncionariosAsync(skip, take);
            return Ok(funcionarios);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Recupera funcionário por ID", Description = "Recupera um funcionário específico pelo ID")]
        [SwaggerResponse(200, "Funcionário recuperado com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Funcionário não encontrado")]
        public async Task<IActionResult> RecuperaFuncionarioPorId(int id)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }
            var funcionario = await _funcionarioService.GetFuncionarioByIdAsync(id);
            if (funcionario == null)
            {
                return NotFound();
            }
            return Ok(funcionario);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Atualiza funcionário por ID", Description = "Atualiza os dados de um funcionário específico pelo ID")]
        [SwaggerResponse(204, "Funcionário atualizado com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Funcionário não encontrado")]
        public async Task<IActionResult> AtualizaFuncionario(int id, [FromBody] UpdateFuncionarioDto funcionarioDto)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }
            var updated = await _funcionarioService.UpdateFuncionarioAsync(id, funcionarioDto);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Deleta funcionário por ID", Description = "Deleta um funcionário específico pelo ID")]
        [SwaggerResponse(204, "Funcionário deletado com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Funcionário não encontrado")]
        public async Task<IActionResult> DeletaFuncionario(int id)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }
            var deleted = await _funcionarioService.DeleteFuncionarioAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
