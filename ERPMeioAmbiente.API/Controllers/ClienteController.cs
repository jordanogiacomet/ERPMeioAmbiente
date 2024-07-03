using AutoMapper;
using ERPMeioAmbienteAPI.Data.Dtos;
using ERPMeioAmbienteAPI.Models;
using ERPMeioAmbienteAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ERPMeioAmbienteAPI.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpPost]
        [Authorize]
        [SwaggerOperation(Summary = "Adiciona um novo cliente", Description = "Adiciona um novo cliente ao sistema")]
        [SwaggerResponse(201, "Cliente criado com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        public async Task<IActionResult> AdicionaCliente([FromBody] CreateClienteDto clienteDto)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }

            var cliente = await _clienteService.AddClienteAsync(clienteDto);
            return CreatedAtAction(nameof(RecuperaClientePorId), new { id = cliente.Id }, cliente);
        }

        [HttpGet]
        [Authorize]
        [SwaggerOperation(Summary = "Recupera todos os clientes", Description = "Recupera uma lista de todos os clientes do sistema")]
        [SwaggerResponse(200, "Lista de clientes recuperada com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        public async Task<IActionResult> RecuperaClientes([FromQuery] int skip = 0, [FromQuery] int take = 50)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }

            var clientes = await _clienteService.GetAllClientesAsync(skip, take);
            return Ok(clientes);
        }

        [HttpGet("me")]
        [Authorize(Policy = "ClientePolicy")]
        [SwaggerOperation(Summary = "Recupera dados do cliente autenticado", Description = "Recupera os dados do cliente atualmente autenticado")]
        [SwaggerResponse(200, "Dados do cliente recuperados com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Cliente não encontrado")]
        public async Task<IActionResult> RecuperaClienteAtual()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var clienteDto = await _clienteService.GetClienteAtualAsync(userId);
            if (clienteDto == null)
            {
                return NotFound();
            }
            return Ok(clienteDto);
        }

        [HttpPut("me")]
        [Authorize(Policy = "ClientePolicy")]
        [SwaggerOperation(Summary = "Atualiza dados do cliente autenticado", Description = "Atualiza os dados do cliente atualmente autenticado")]
        [SwaggerResponse(200, "Dados do cliente atualizados com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Cliente não encontrado")]
        public async Task<IActionResult> AtualizaClienteAtual([FromBody] UpdateClienteDto clienteDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var updatedCliente = await _clienteService.UpdateClienteAtualAsync(userId, clienteDto);
            if (updatedCliente == null)
            {
                return NotFound();
            }
            return Ok(updatedCliente);
        }

        [HttpGet("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Recupera cliente por ID", Description = "Recupera um cliente específico pelo ID")]
        [SwaggerResponse(200, "Cliente recuperado com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Cliente não encontrado")]
        public async Task<IActionResult> RecuperaClientePorId(int id)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }

            var clienteDto = await _clienteService.GetClienteByIdAsync(id);
            if (clienteDto == null)
            {
                return NotFound();
            }
            return Ok(clienteDto);
        }

        [HttpPut("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Atualiza cliente por ID", Description = "Atualiza os dados de um cliente específico pelo ID")]
        [SwaggerResponse(204, "Cliente atualizado com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Cliente não encontrado")]
        public async Task<IActionResult> AtualizaCliente(int id, [FromBody] UpdateClienteDto clienteDto)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }

            var updated = await _clienteService.UpdateClienteAsync(id, clienteDto);
            if (!updated)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Deleta cliente por ID", Description = "Deleta um cliente específico pelo ID")]
        [SwaggerResponse(204, "Cliente deletado com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Cliente não encontrado")]
        public async Task<IActionResult> DeletaCliente(int id)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }

            var deleted = await _clienteService.DeleteClienteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPost("me/solicita-coleta")]
        [Authorize(Policy = "ClientePolicy")]
        [SwaggerOperation(Summary = "Solicita uma coleta", Description = "Permite que o cliente solicite uma nova coleta")]
        [SwaggerResponse(201, "Coleta solicitada com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        public async Task<IActionResult> SolicitaColeta([FromBody] CreateColetaForClienteDto coletaDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var readColetaDto = await _clienteService.SolicitaColetaAsync(userId, coletaDto);
            if (readColetaDto == null)
            {
                return NotFound();
            }

            return CreatedAtAction("RecuperaColetaPorId", "Coleta", new { id = readColetaDto.Id }, readColetaDto);
        }

        [HttpGet("me/coletas")]
        [Authorize(Policy = "ClientePolicy")]
        [SwaggerOperation(Summary = "Recupera todas as coletas do cliente", Description = "Recupera todas as coletas solicitadas pelo cliente atualmente autenticado")]
        [SwaggerResponse(200, "Lista de coletas recuperada com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Cliente não encontrado")]
        public async Task<IActionResult> RecuperaColetasDoCliente()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var coletasDto = await _clienteService.GetColetasDoClienteAsync(userId);
            if (coletasDto == null)
            {
                return NotFound();
            }
            return Ok(coletasDto);
        }

        [HttpGet("me/coletas/{id}")]
        [Authorize(Policy = "ClientePolicy")]
        [SwaggerOperation(Summary = "Recupera uma coleta do cliente por ID", Description = "Recupera uma coleta específica solicitada pelo cliente atualmente autenticado")]
        [SwaggerResponse(200, "Coleta recuperada com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Coleta não encontrada")]
        public async Task<IActionResult> RecuperaColetaDoClientePorId(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var coletaDto = await _clienteService.GetColetaDoClientePorIdAsync(userId, id);
            if (coletaDto == null)
            {
                return NotFound();
            }
            return Ok(coletaDto);
        }

        [HttpPut("me/coletas/{id}")]
        [Authorize(Policy = "ClientePolicy")]
        [SwaggerOperation(Summary = "Atualiza uma coleta do cliente", Description = "Atualiza uma coleta específica solicitada pelo cliente atualmente autenticado")]
        [SwaggerResponse(200, "Coleta atualizada com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Coleta não encontrada")]
        public async Task<IActionResult> AtualizaColetaDoCliente(int id, [FromBody] UpdateColetaDto coletaDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var updated = await _clienteService.UpdateColetaDoClienteAsync(userId, id, coletaDto);
            if (!updated)
            {
                return NotFound();
            }
            return Ok();
        }

        [HttpDelete("me/coletas/{id}")]
        [Authorize(Policy = "ClientePolicy")]
        [SwaggerOperation(Summary = "Deleta uma coleta do cliente", Description = "Deleta uma coleta específica solicitada pelo cliente atualmente autenticado")]
        [SwaggerResponse(204, "Coleta deletada com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Coleta não encontrada")]
        public async Task<IActionResult> DeletaColetaDoCliente(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var deleted = await _clienteService.DeleteColetaDoClienteAsync(userId, id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
