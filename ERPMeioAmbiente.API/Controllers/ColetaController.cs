using AutoMapper;
using ERPMeioAmbienteAPI.Data.Dtos;
using ERPMeioAmbienteAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPMeioAmbienteAPI.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize]
    public class ColetaController : ControllerBase
    {
        private readonly IColetaService _coletaService;

        public ColetaController(IColetaService coletaService)
        {
            _coletaService = coletaService;
        }

        [HttpPost]
        [Authorize]
        [SwaggerOperation(Summary = "Adiciona uma nova coleta", Description = "Adiciona uma nova coleta ao sistema")]
        [SwaggerResponse(201, "Coleta criada com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(400, "Erro de validação ou criação")]
        public async Task<IActionResult> AdicionaColeta([FromBody] CreateColetaDto coletaDto)
        {
            if (User.IsInRole("Cliente")) return Unauthorized();

            try
            {
                var coleta = await _coletaService.AddColetaAsync(coletaDto);
                return CreatedAtAction(nameof(RecuperaColetaPorId), new { id = coleta.Id }, coleta);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno ao adicionar coleta.", details = ex.Message });
            }
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Recupera todas as coletas", Description = "Recupera uma lista de todas as coletas do sistema")]
        [SwaggerResponse(200, "Lista de coletas recuperada com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(500, "Erro interno")]
        public async Task<IActionResult> RecuperaColetas([FromQuery] int skip = 0, [FromQuery] int take = 50)
        {
            if (User.IsInRole("Cliente")) return Unauthorized();

            try
            {
                var coletas = await _coletaService.GetAllColetasAsync(skip, take);
                return Ok(coletas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno ao recuperar coletas.", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Recupera coleta por ID", Description = "Recupera uma coleta específica pelo ID")]
        [SwaggerResponse(200, "Coleta recuperada com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Coleta não encontrada")]
        [SwaggerResponse(500, "Erro interno")]
        public async Task<IActionResult> RecuperaColetaPorId(int id)
        {
            try
            {
                var coleta = await _coletaService.GetColetaByIdAsync(id);
                if (coleta == null) return NotFound();
                return Ok(coleta);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno ao recuperar coleta.", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Atualiza coleta por ID", Description = "Atualiza os dados de uma coleta específica pelo ID")]
        [SwaggerResponse(204, "Coleta atualizada com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Coleta não encontrada")]
        [SwaggerResponse(500, "Erro interno")]
        public async Task<IActionResult> AtualizaColeta(int id, [FromBody] UpdateColetaDto coletaDto)
        {
            if (User.IsInRole("Cliente")) return Unauthorized();

            try
            {
                var updated = await _coletaService.UpdateColetaAsync(id, coletaDto);
                if (!updated) return NotFound();
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno ao atualizar coleta.", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Deleta coleta por ID", Description = "Deleta uma coleta específica pelo ID")]
        [SwaggerResponse(204, "Coleta deletada com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Coleta não encontrada")]
        [SwaggerResponse(500, "Erro interno")]
        public async Task<IActionResult> DeletaColeta(int id)
        {
            if (User.IsInRole("Cliente")) return Unauthorized();

            try
            {
                var deleted = await _coletaService.DeleteColetaAsync(id);
                if (!deleted) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno ao deletar coleta.", details = ex.Message });
            }
        }
    }
}
