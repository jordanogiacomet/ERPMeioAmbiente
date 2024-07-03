using AutoMapper;
using ERPMeioAmbienteAPI.Data.Dtos;
using ERPMeioAmbienteAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPMeioAmbienteAPI.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class ResiduoController : ControllerBase
    {
        private readonly IResiduoService _residuoService;
        private readonly IMapper _mapper;

        public ResiduoController(IResiduoService residuoService, IMapper mapper)
        {
            _residuoService = residuoService;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize]
        [SwaggerOperation(Summary = "Adiciona um novo resíduo", Description = "Adiciona um novo resíduo ao sistema")]
        [SwaggerResponse(201, "Resíduo criado com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        public async Task<IActionResult> AdicionaResiduo([FromBody] CreateResiduoDto residuoDto)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }
            var residuo = await _residuoService.AddResiduoAsync(residuoDto);
            return CreatedAtAction(nameof(RecuperaResiduoPorId), new { id = residuo.Id }, residuo);
        }

        [HttpGet]
        [Authorize]
        [SwaggerOperation(Summary = "Recupera todos os resíduos", Description = "Recupera uma lista de todos os resíduos do sistema")]
        [SwaggerResponse(200, "Lista de resíduos recuperada com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        public async Task<IActionResult> RecuperaResiduos([FromQuery] int skip = 0, [FromQuery] int take = 50)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }
            var residuos = await _residuoService.GetAllResiduosAsync(skip, take);
            return Ok(residuos);
        }

        [HttpGet("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Recupera resíduo por ID", Description = "Recupera um resíduo específico pelo ID")]
        [SwaggerResponse(200, "Resíduo recuperado com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Resíduo não encontrado")]
        public async Task<IActionResult> RecuperaResiduoPorId(int id)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }
            var residuo = await _residuoService.GetResiduoByIdAsync(id);
            if (residuo == null)
            {
                return NotFound();
            }
            return Ok(residuo);
        }

        [HttpPut("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Atualiza resíduo por ID", Description = "Atualiza os dados de um resíduo específico pelo ID")]
        [SwaggerResponse(204, "Resíduo atualizado com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Resíduo não encontrado")]
        public async Task<IActionResult> AtualizaResiduo(int id, [FromBody] UpdateResiduoDto residuoDto)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }
            var updated = await _residuoService.UpdateResiduoAsync(id, residuoDto);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Deleta resíduo por ID", Description = "Deleta um resíduo específico pelo ID")]
        [SwaggerResponse(204, "Resíduo deletado com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Resíduo não encontrado")]
        public async Task<IActionResult> DeletaResiduo(int id)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }
            var deleted = await _residuoService.DeleteResiduoAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
