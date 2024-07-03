using AutoMapper;
using ERPMeioAmbienteAPI.Data.Dtos;
using ERPMeioAmbienteAPI.Models;
using ERPMeioAmbienteAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPMeioAmbienteAPI.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize]
    public class AgendamentoController : ControllerBase
    {
        private readonly IAgendamentoService _agendamentoService;

        public AgendamentoController(IAgendamentoService agendamentoService)
        {
            _agendamentoService = agendamentoService;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Adiciona um novo agendamento", Description = "Adiciona um novo agendamento para uma coleta")]
        [SwaggerResponse(201, "Agendamento criado com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        public async Task<IActionResult> AdicionaAgendamento([FromBody] CreateAgendamentoDto agendamentoDto)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }
            var agendamento = await _agendamentoService.AddAgendamentoAsync(agendamentoDto);
            return CreatedAtAction(nameof(RecuperaAgendamentoPorId), new { id = agendamento.Id }, agendamento);
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Recupera todos os agendamentos", Description = "Recupera uma lista de todos os agendamentos do sistema")]
        [SwaggerResponse(200, "Lista de agendamentos recuperada com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        public async Task<IActionResult> RecuperaAgendamentos([FromQuery] int skip = 0, [FromQuery] int take = 50)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }
            var agendamentos = await _agendamentoService.GetAllAgendamentosAsync(skip, take);
            return Ok(agendamentos);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Recupera agendamento por ID", Description = "Recupera um agendamento específico pelo ID")]
        [SwaggerResponse(200, "Agendamento recuperado com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Agendamento não encontrado")]
        public async Task<IActionResult> RecuperaAgendamentoPorId(int id)
        {
            var agendamentoDto = await _agendamentoService.GetAgendamentoByIdAsync(id);
            if (agendamentoDto == null)
            {
                return NotFound();
            }
            return Ok(agendamentoDto);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Atualiza agendamento por ID", Description = "Atualiza os dados de um agendamento específico pelo ID")]
        [SwaggerResponse(204, "Agendamento atualizado com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Agendamento não encontrado")]
        public async Task<IActionResult> AtualizaAgendamento(int id, [FromBody] UpdateAgendamentoDto agendamentoDto)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }

            var updated = await _agendamentoService.UpdateAgendamentoAsync(id, agendamentoDto);
            if (!updated)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Deleta agendamento por ID", Description = "Deleta um agendamento específico pelo ID")]
        [SwaggerResponse(204, "Agendamento deletado com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Agendamento não encontrado")]
        public async Task<IActionResult> DeletaAgendamento(int id)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }

            var deleted = await _agendamentoService.DeleteAgendamentoAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
