using AutoMapper;
using ERPMeioAmbiente.API.Models;
using ERPMeioAmbienteAPI.Data;
using ERPMeioAmbienteAPI.Data.Dtos;
using ERPMeioAmbienteAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;

namespace ERPMeioAmbienteAPI.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize(Policy = "FuncionarioPolicy, AdminPolicy")] // Proteger todas as rotas
    public class ColetaController : ControllerBase
    {
        private readonly ERPMeioAmbienteContext _context;
        private readonly IMapper _mapper;

        public ColetaController(ERPMeioAmbienteContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize]
        [SwaggerOperation(Summary = "Adiciona uma nova coleta", Description = "Adiciona uma nova coleta ao sistema")]
        [SwaggerResponse(201, "Coleta criada com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        public IActionResult AdicionaColeta([FromBody] CreateColetaDto coletaDto)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }
            Coleta coleta = _mapper.Map<Coleta>(coletaDto);
            _context.Coletas.Add(coleta);
            _context.SaveChanges();
            return CreatedAtAction(nameof(RecuperaColetaPorId), new { id = coleta.Id }, coleta);
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Recupera todas as coletas", Description = "Recupera uma lista de todas as coletas do sistema")]
        [SwaggerResponse(200, "Lista de coletas recuperada com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        public IActionResult RecuperaColetas([FromQuery] int skip = 0, [FromQuery] int take = 50)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }
            var coletas = _mapper.Map<List<ReadColetaDto>>(_context.Coletas.Skip(skip).Take(take).ToList());
            return Ok(coletas);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Recupera coleta por ID", Description = "Recupera uma coleta específica pelo ID")]
        [SwaggerResponse(200, "Coleta recuperada com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Coleta não encontrada")]
        public IActionResult RecuperaColetaPorId(int id)
        {
            var coleta = _context.Coletas.FirstOrDefault(coleta => coleta.Id == id);
            if (coleta == null)
            {
                return NotFound();
            }
            var coletaDto = _mapper.Map<ReadColetaDto>(coleta);
            return Ok(coletaDto);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Atualiza coleta por ID", Description = "Atualiza os dados de uma coleta específica pelo ID")]
        [SwaggerResponse(204, "Coleta atualizada com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Coleta não encontrada")]
        public IActionResult AtualizaColeta(int id, [FromBody] UpdateColetaDto coletaDto)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }
            var coleta = _context.Coletas.FirstOrDefault(coleta => coleta.Id == id);
            if (coleta == null) return NotFound();
            _mapper.Map(coletaDto, coleta);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Deleta coleta por ID", Description = "Deleta uma coleta específica pelo ID")]
        [SwaggerResponse(204, "Coleta deletada com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Coleta não encontrada")]
        public IActionResult DeletaColeta(int id)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }
            var coleta = _context.Coletas.FirstOrDefault(coleta => coleta.Id == id);
            if (coleta == null) return NotFound();
            _context.Remove(coleta);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
