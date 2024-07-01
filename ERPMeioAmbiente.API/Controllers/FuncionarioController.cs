using AutoMapper;
using ERPMeioAmbiente.API.Data.Dtos;
using ERPMeioAmbienteAPI.Data;
using ERPMeioAmbienteAPI.Data.Dtos;
using ERPMeioAmbienteAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace ERPMeioAmbiente.API.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize] // Proteger todas as rotas
    public class FuncionarioController : ControllerBase
    {
        private readonly ERPMeioAmbienteContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;

        public FuncionarioController(ERPMeioAmbienteContext context, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
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

            // Criar usuário Identity
            var identityUser = new IdentityUser
            {
                Email = funcionarioDto.Email,
                UserName = funcionarioDto.Email
            };

            var result = await _userManager.CreateAsync(identityUser, funcionarioDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Erro ao criar usuário", errors = result.Errors.Select(e => e.Description) });
            }

            await _userManager.AddToRoleAsync(identityUser, "Funcionario");

            // Criar funcionário
            Funcionario funcionario = _mapper.Map<Funcionario>(funcionarioDto);
            funcionario.UserId = identityUser.Id;

            _context.Funcionarios.Add(funcionario);
            _context.SaveChanges();

            return CreatedAtAction(nameof(RecuperaFuncionarioPorId), new { id = funcionario.Id }, funcionario);
        }

        [HttpGet]
        [Authorize]
        [SwaggerOperation(Summary = "Recupera todos os funcionários", Description = "Recupera uma lista de todos os funcionários do sistema")]
        [SwaggerResponse(200, "Lista de funcionários recuperada com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        public IActionResult RecuperaFuncionarios([FromQuery] int skip = 0, [FromQuery] int take = 50)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }
            var funcionarios = _mapper.Map<List<ReadFuncionarioDto>>(_context.Funcionarios.Skip(skip).Take(take).ToList());
            return Ok(funcionarios);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Recupera funcionário por ID", Description = "Recupera um funcionário específico pelo ID")]
        [SwaggerResponse(200, "Funcionário recuperado com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Funcionário não encontrado")]
        public IActionResult RecuperaFuncionarioPorId(int id)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }
            var funcionario = _context.Funcionarios.FirstOrDefault(funcionario => funcionario.Id == id);
            if (funcionario == null)
            {
                return NotFound();
            }
            var funcionarioDto = _mapper.Map<ReadFuncionarioDto>(funcionario);
            return Ok(funcionarioDto);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Atualiza funcionário por ID", Description = "Atualiza os dados de um funcionário específico pelo ID")]
        [SwaggerResponse(204, "Funcionário atualizado com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Funcionário não encontrado")]
        public IActionResult AtualizaFuncionario(int id, [FromBody] UpdateFuncionarioDto funcionarioDto)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }
            var funcionario = _context.Funcionarios.FirstOrDefault(funcionario => funcionario.Id == id);
            if (funcionario == null) return NotFound();
            _mapper.Map(funcionarioDto, funcionario);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Deleta funcionário por ID", Description = "Deleta um funcionário específico pelo ID")]
        [SwaggerResponse(204, "Funcionário deletado com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Funcionário não encontrado")]
        public IActionResult DeletaFuncionario(int id)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }
            var funcionario = _context.Funcionarios.FirstOrDefault(funcionario => funcionario.Id == id);
            if (funcionario == null) return NotFound();
            _context.Remove(funcionario);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
