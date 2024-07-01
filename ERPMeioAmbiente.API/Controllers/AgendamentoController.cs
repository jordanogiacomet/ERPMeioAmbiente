using AutoMapper;
using ERPMeioAmbiente.API.Data.Dtos;
using ERPMeioAmbienteAPI.Data.Dtos;
using ERPMeioAmbienteAPI.Data;
using ERPMeioAmbienteAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.EntityFrameworkCore;

namespace ERPMeioAmbiente.API.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize] // Proteger todas as rotas
    public class AgendamentoController : ControllerBase
    {
        private readonly ERPMeioAmbienteContext _context;
        private readonly IMapper _mapper;

        public AgendamentoController(ERPMeioAmbienteContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Adiciona um novo agendamento", Description = "Adiciona um novo agendamento para uma coleta")]
        [SwaggerResponse(201, "Agendamento criado com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        public IActionResult AdicionaAgendamento([FromBody] CreateAgendamentoDto agendamentoDto)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }
            Agendamento agendamento = _mapper.Map<Agendamento>(agendamentoDto);
            _context.Agendamentos.Add(agendamento);
            _context.SaveChanges();
            return CreatedAtAction(nameof(RecuperaAgendamentoPorId), new { id = agendamento.Id }, agendamento);
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Recupera todos os agendamentos", Description = "Recupera uma lista de todos os agendamentos do sistema")]
        [SwaggerResponse(200, "Lista de agendamentos recuperada com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        public IActionResult RecuperaAgendamentos([FromQuery] int skip = 0, [FromQuery] int take = 50)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }
            var agendamentos = _mapper.Map<List<ReadAgendamentoDto>>(_context.Agendamentos.Skip(skip).Take(take).ToList());
            return Ok(agendamentos);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Recupera agendamento por ID", Description = "Recupera um agendamento específico pelo ID")]
        [SwaggerResponse(200, "Agendamento recuperado com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Agendamento não encontrado")]
        public IActionResult RecuperaAgendamentoPorId(int id)
        {
            var agendamento = _context.Agendamentos.FirstOrDefault(agendamento => agendamento.Id == id);
            if (agendamento == null)
            {
                return NotFound();
            }
            var agendamentoDto = _mapper.Map<ReadAgendamentoDto>(agendamento);
            return Ok(agendamentoDto);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Atualiza agendamento por ID", Description = "Atualiza os dados de um agendamento específico pelo ID")]
        [SwaggerResponse(204, "Agendamento atualizado com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Agendamento não encontrado")]
        public IActionResult AtualizaAgendamento(int id, [FromBody] UpdateAgendamentoDto agendamentoDto)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }

            var agendamento = _context.Agendamentos.FirstOrDefault(a => a.Id == id);
            if (agendamento == null)
                return NotFound("Agendamento não encontrado");

            var coleta = _context.Coletas.FirstOrDefault(c => c.Id == agendamentoDto.ColetaId);
            if (coleta == null)
                return BadRequest("Coleta associada não encontrada");

            _mapper.Map(agendamentoDto, agendamento);

            agendamento.Coleta = coleta; // Associar a coleta corretamente

            try
            {
                _context.SaveChanges();
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Erro ao atualizar o agendamento: {ex.InnerException?.Message ?? ex.Message}");
            }
        }


        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Deleta agendamento por ID", Description = "Deleta um agendamento específico pelo ID")]
        [SwaggerResponse(204, "Agendamento deletado com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Agendamento não encontrado")]
        public IActionResult DeletaAgendamento(int id)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }
            var agendamento = _context.Agendamentos.FirstOrDefault(agendamento => agendamento.Id == id);
            if (agendamento == null) return NotFound();
            _context.Remove(agendamento);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
