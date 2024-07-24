using AutoMapper;
using ERPMeioAmbiente.API.Services;
using ERPMeioAmbienteAPI.Data.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;

namespace ERPMeioAmbiente.API.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize]
    public class MotoristaController : ControllerBase
    {
        private readonly IMotoristaService _motoristaService;
        private readonly IMapper _mapper;

        public MotoristaController(IMotoristaService motoristaService, IMapper mapper)
        {
            _motoristaService = motoristaService;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize]
        [SwaggerOperation(Summary = "Adiciona um novo motorista", Description = "Adiciona um novo motorista ao sistema")]
        [SwaggerResponse(201, "Motorista criado com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(400, "Erro de validação ou criação")]
        public async Task<IActionResult> AdicionaMotorista([FromBody] CreateMotoristaDto motoristaDto)
        {
            if (User.IsInRole("Cliente")) return Unauthorized();

            try
            {
                var motorista = await _motoristaService.AddMotoristaAsync(motoristaDto);
                return CreatedAtAction(nameof(RecuperaMotoristaPorId), new { id = motorista.Id }, motorista);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocorreu um erro interno.", details = ex.Message });
            }
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Recupera todos os motoristas", Description = "Recupera uma lista de todos os motoristas do sistema")]
        [SwaggerResponse(200, "Lista de motoristas recuperada com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(500, "Erro interno")]
        public async Task<IActionResult> RecuperaMotoristas([FromQuery] int skip, int take)
        {
            if (User.IsInRole("Cliente")) return Unauthorized();

            try
            {
                var motoristas = await _motoristaService.GetAllMotoristasAsync(skip, take);
                return Ok(motoristas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocorreu um erro interno.", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Recupera motorista por ID", Description = "Recupera um motorista específico pelo ID")]
        [SwaggerResponse(200, "Motorista recuperado com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Motorista não encontrado")]
        [SwaggerResponse(500, "Erro interno")]
        public async Task<IActionResult> RecuperaMotoristaPorId(int id)
        {
            if (User.IsInRole("Cliente")) return Unauthorized();

            try
            {
                var motorista = await _motoristaService.GetMotoristaById(id);
                if (motorista == null) return NotFound();
                return Ok(motorista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocorreu um erro interno.", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Atualiza motorista por ID", Description = "Atualiza os dados de um motorista específico pelo ID")]
        [SwaggerResponse(204, "Motorista atualizado com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Motorista não encontrado")]
        [SwaggerResponse(500, "Erro interno")]
        public async Task<IActionResult> AtualizaMotoristaPorId(int id, [FromBody] UpdateMotoristaDto motoristaDto)
        {
            if (User.IsInRole("Cliente")) return Unauthorized();

            try
            {
                var updated = await _motoristaService.UpdateMotoristaAsync(id, motoristaDto);
                if (!updated) return NotFound();
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocorreu um erro interno.", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Deleta motorista por ID", Description = "Deleta um motorista específico pelo ID")]
        [SwaggerResponse(204, "Motorista deletado com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Motorista não encontrado")]
        [SwaggerResponse(500, "Erro interno")]
        public async Task<IActionResult> DeletaMotoristaPorId(int id)
        {
            if (User.IsInRole("Cliente")) return Unauthorized();

            try
            {
                var deleted = await _motoristaService.DeleteMotoristaAsync(id);
                if (!deleted) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocorreu um erro interno.", details = ex.Message });
            }
        }
    }
}
