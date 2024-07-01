using AutoMapper;
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
    public class ResiduoController : ControllerBase
    {
        private readonly ERPMeioAmbienteContext _context;
        private readonly IMapper _mapper;

        public ResiduoController(ERPMeioAmbienteContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize]
        [SwaggerOperation(Summary = "Adiciona um novo resíduo", Description = "Adiciona um novo resíduo ao sistema")]
        [SwaggerResponse(201, "Resíduo criado com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        public IActionResult AdicionaResiduo([FromBody] CreateResiduoDto residuoDto)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }
            Residuo residuo = _mapper.Map<Residuo>(residuoDto);
            _context.Residuos.Add(residuo);
            _context.SaveChanges();
            return CreatedAtAction(nameof(RecuperaResiduoPorId), new { id = residuo.Id }, residuo);
        }

        [HttpGet]
        [Authorize]
        [SwaggerOperation(Summary = "Recupera todos os resíduos", Description = "Recupera uma lista de todos os resíduos do sistema")]
        [SwaggerResponse(200, "Lista de resíduos recuperada com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        public IActionResult RecuperaResiduos([FromQuery] int skip = 0, [FromQuery] int take = 50)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }
            var residuos = _mapper.Map<List<ReadResiduoDto>>(_context.Residuos.Skip(skip).Take(take).ToList());
            return Ok(residuos);
        }

        [HttpGet("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Recupera resíduo por ID", Description = "Recupera um resíduo específico pelo ID")]
        [SwaggerResponse(200, "Resíduo recuperado com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Resíduo não encontrado")]
        public IActionResult RecuperaResiduoPorId(int id)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }
            var residuo = _context.Residuos.FirstOrDefault(residuo => residuo.Id == id);
            if (residuo == null)
            {
                return NotFound();
            }
            var residuoDto = _mapper.Map<ReadResiduoDto>(residuo);
            return Ok(residuoDto);
        }

        [HttpPut("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Atualiza resíduo por ID", Description = "Atualiza os dados de um resíduo específico pelo ID")]
        [SwaggerResponse(204, "Resíduo atualizado com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Resíduo não encontrado")]
        public IActionResult AtualizaResiduo(int id, [FromBody] UpdateResiduoDto residuoDto)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }
            var residuo = _context.Residuos.FirstOrDefault(residuo => residuo.Id == id);
            if (residuo == null) return NotFound();
            _mapper.Map(residuoDto, residuo);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Deleta resíduo por ID", Description = "Deleta um resíduo específico pelo ID")]
        [SwaggerResponse(204, "Resíduo deletado com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Resíduo não encontrado")]
        public IActionResult DeletaResiduo(int id)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }
            var residuo = _context.Residuos.FirstOrDefault(residuo => residuo.Id == id);
            if (residuo == null) return NotFound();
            _context.Remove(residuo);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
