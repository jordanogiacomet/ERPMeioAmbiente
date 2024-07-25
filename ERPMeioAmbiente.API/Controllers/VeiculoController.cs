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
    public class VeiculoController : ControllerBase
    {
        private readonly IVeiculoService _veiculoService;
        private readonly IMapper _mapper;

        public VeiculoController(IVeiculoService veiculoService, IMapper mapper)
        {
            _veiculoService = veiculoService;
            _mapper = mapper;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Adiciona um novo veículo", Description = "Adiciona um novo veículo ao sistema")]
        [SwaggerResponse(201, "Veículo criado com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(400, "Erro de validação ou criação")]
        public async Task<IActionResult> AdicionaVeiculo([FromBody] CreateVeiculoDto veiculoDto)
        {
            try
            {
                var veiculo = await _veiculoService.AddVeiculoAsync(veiculoDto);
                return CreatedAtAction(nameof(RecuperaVeiculoPorId), new { id = veiculo.Id }, veiculo);
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
        [SwaggerOperation(Summary = "Recupera todos os veículos", Description = "Recupera uma lista de todos os veículos do sistema")]
        [SwaggerResponse(200, "Lista de veículos recuperada com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(500, "Erro interno")]
        public async Task<IActionResult> RecuperaVeiculos([FromQuery] int skip = 0, int take = 50)
        {
            try
            {
                var veiculos = await _veiculoService.GetAllVeiculosAsync(skip, take);
                return Ok(veiculos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocorreu um erro interno.", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Recupera veículo por ID", Description = "Recupera um veículo específico pelo ID")]
        [SwaggerResponse(200, "Veículo recuperado com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Veículo não encontrado")]
        [SwaggerResponse(500, "Erro interno")]
        public async Task<IActionResult> RecuperaVeiculoPorId(int id)
        {
            try
            {
                var veiculo = await _veiculoService.GetVeiculoByIdAsync(id);
                if (veiculo == null) return NotFound();
                return Ok(veiculo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocorreu um erro interno.", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Atualiza veículo por ID", Description = "Atualiza os dados de um veículo específico pelo ID")]
        [SwaggerResponse(204, "Veículo atualizado com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Veículo não encontrado")]
        [SwaggerResponse(500, "Erro interno")]
        public async Task<IActionResult> AtualizaVeiculoPorId(int id, [FromBody] UpdateVeiculoDto veiculoDto)
        {
            try
            {
                var updated = await _veiculoService.UpdateVeiculoAsync(id, veiculoDto);
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
        [SwaggerOperation(Summary = "Deleta veículo por ID", Description = "Deleta um veículo específico pelo ID")]
        [SwaggerResponse(204, "Veículo deletado com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Veículo não encontrado")]
        [SwaggerResponse(500, "Erro interno")]
        public async Task<IActionResult> DeletaVeiculoPorId(int id)
        {
            try
            {
                var deleted = await _veiculoService.DeleteVeiculoAsync(id);
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
