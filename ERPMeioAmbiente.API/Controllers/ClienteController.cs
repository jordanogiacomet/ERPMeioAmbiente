using AutoMapper;
using ERPMeioAmbiente.API.Models;
using ERPMeioAmbienteAPI.Data;
using ERPMeioAmbienteAPI.Data.Dtos;
using ERPMeioAmbienteAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace ERPMeioAmbienteAPI.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize]
    public class ClienteController : ControllerBase
    {
        private readonly ERPMeioAmbienteContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;

        public ClienteController(ERPMeioAmbienteContext context, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
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

            Cliente cliente = _mapper.Map<Cliente>(clienteDto);

            // Se o email e senha foram fornecidos, criar um usuário para o cliente
            if (!string.IsNullOrEmpty(clienteDto.Email) && !string.IsNullOrEmpty(clienteDto.Password))
            {
                var identityUser = new IdentityUser
                {
                    Email = clienteDto.Email,
                    UserName = clienteDto.Email,
                };

                var result = await _userManager.CreateAsync(identityUser, clienteDto.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(identityUser, "Cliente");
                    cliente.UserId = identityUser.Id;
                }
                else
                {
                    return BadRequest(new { message = "Erro ao criar usuário", errors = result.Errors });
                }
            }

            _context.Clientes.Add(cliente);
            _context.SaveChanges();
            return CreatedAtAction(nameof(RecuperaClientePorId), new { id = cliente.Id }, cliente);
        }

        [HttpGet]
        [Authorize]
        [SwaggerOperation(Summary = "Recupera todos os clientes", Description = "Recupera uma lista de todos os clientes do sistema")]
        [SwaggerResponse(200, "Lista de clientes recuperada com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        public IActionResult RecuperaClientes([FromQuery] int skip = 0, [FromQuery] int take = 50)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }

            var clientes = _mapper.Map<List<ReadClienteDto>>(_context.Clientes.Skip(skip).Take(take).ToList());
            return Ok(clientes);
        }

        [HttpGet("me")]
        [Authorize(Policy = "ClientePolicy")]
        [SwaggerOperation(Summary = "Recupera dados do cliente autenticado", Description = "Recupera os dados do cliente atualmente autenticado")]
        [SwaggerResponse(200, "Dados do cliente recuperados com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Cliente não encontrado")]
        public IActionResult RecuperaClienteAtual()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var cliente = _context.Clientes.FirstOrDefault(c => c.UserId == userId);
            if (cliente == null)
            {
                return NotFound();
            }

            var clienteDto = _mapper.Map<ReadClienteDto>(cliente);
            return Ok(clienteDto);
        }

        [HttpPut("me")]
        [Authorize(Policy = "ClientePolicy")]
        [SwaggerOperation(Summary = "Atualiza dados do cliente autenticado", Description = "Atualiza os dados do cliente atualmente autenticado")]
        [SwaggerResponse(200, "Dados do cliente atualizados com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Cliente não encontrado")]
        public IActionResult AtualizaClienteAtual([FromBody] UpdateClienteDto clienteDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var cliente = _context.Clientes.FirstOrDefault(c => c.UserId == userId);
            if (cliente == null)
            {
                return NotFound();
            }

            _mapper.Map(clienteDto, cliente);
            _context.SaveChanges();
            return Ok(_mapper.Map<ReadClienteDto>(cliente));
        }

        [HttpGet("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Recupera cliente por ID", Description = "Recupera um cliente específico pelo ID")]
        [SwaggerResponse(200, "Cliente recuperado com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Cliente não encontrado")]
        public IActionResult RecuperaClientePorId(int id)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }
            var cliente = _context.Clientes.FirstOrDefault(cliente => cliente.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }

            var clienteDto = _mapper.Map<ReadClienteDto>(cliente);
            return Ok(clienteDto);
        }

        [HttpPut("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Atualiza cliente por ID", Description = "Atualiza os dados de um cliente específico pelo ID")]
        [SwaggerResponse(204, "Cliente atualizado com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Cliente não encontrado")]
        public IActionResult AtualizaCliente(int id, [FromBody] UpdateClienteDto clienteDto)
        {
            if (User.IsInRole("Cliente"))
            {
                return Unauthorized();
            }
            var cliente = _context.Clientes.FirstOrDefault(c => c.Id == id);
            if (cliente == null) return NotFound();

            _mapper.Map(clienteDto, cliente);
            _context.SaveChanges();
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

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var cliente = await _context.Clientes.Include(c => c.User).FirstOrDefaultAsync(c => c.Id == id);
                    if (cliente == null)
                    {
                        return NotFound();
                    }

                    var user = await _userManager.FindByIdAsync(cliente.UserId);
                    if (user == null)
                    {
                        return NotFound();
                    }

                    _context.Clientes.Remove(cliente);
                    var userResult = await _userManager.DeleteAsync(user);
                    if (!userResult.Succeeded)
                    {
                        throw new Exception(string.Join(", ", userResult.Errors.Select(e => e.Description)));
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return NoContent();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    await transaction.RollbackAsync();
                    return Conflict("Concurrency conflict occurred while trying to delete the client. Please try again.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }
        }

        [HttpPost("me/solicita-coleta")]
        [Authorize(Policy = "ClientePolicy")]
        [SwaggerOperation(Summary = "Solicita uma coleta", Description = "Permite que o cliente solicite uma nova coleta")]
        [SwaggerResponse(201, "Coleta solicitada com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        public IActionResult SolicitaColeta([FromBody] CreateColetaForClienteDto coletaDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var cliente = _context.Clientes.FirstOrDefault(c => c.UserId == userId);
            if (cliente == null)
            {
                return NotFound();
            }

            var coleta = _mapper.Map<Coleta>(coletaDto);
            coleta.ClienteId = cliente.Id;
            coleta.Cliente = cliente;
            coleta.ColetaResiduos = new List<ColetaResiduo>();

            foreach (var residuoId in coletaDto.ResiduoIds)
            {
                coleta.ColetaResiduos.Add(new ColetaResiduo { ResiduoId = residuoId });
            }

            _context.Coletas.Add(coleta);
            _context.SaveChanges();

            var readColetaDto = _mapper.Map<ReadColetaDto>(coleta);

            return CreatedAtAction("RecuperaColetaPorId", "Coleta", new { id = coleta.Id }, readColetaDto);
        }


        [HttpGet("me/coletas")]
        [Authorize(Policy = "ClientePolicy")]
        [SwaggerOperation(Summary = "Recupera todas as coletas do cliente", Description = "Recupera todas as coletas solicitadas pelo cliente atualmente autenticado")]
        [SwaggerResponse(200, "Lista de coletas recuperada com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Cliente não encontrado")]
        public IActionResult RecuperaColetasDoCliente()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var cliente = _context.Clientes.FirstOrDefault(c => c.UserId == userId);
            if (cliente == null)
            {
                return NotFound();
            }

            var coletas = _context.Coletas
                .Include(c => c.ColetaResiduos)
                .ThenInclude(cr => cr.Residuo)
                .Where(c => c.ClienteId == cliente.Id)
                .ToList();
            var coletasDto = _mapper.Map<List<ReadColetaDto>>(coletas);
            return Ok(coletasDto);
        }

        [HttpGet("me/coletas/{id}")]
        [Authorize(Policy = "ClientePolicy")]
        [SwaggerOperation(Summary = "Recupera uma coleta do cliente por ID", Description = "Recupera uma coleta específica solicitada pelo cliente atualmente autenticado")]
        [SwaggerResponse(200, "Coleta recuperada com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Coleta não encontrada")]
        public IActionResult RecuperaColetaDoClientePorId(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var cliente = _context.Clientes.FirstOrDefault(c => c.UserId == userId);
            if (cliente == null)
            {
                return NotFound();
            }

            var coleta = _context.Coletas
                .Include(c => c.ColetaResiduos)
                .ThenInclude(cr => cr.Residuo)
                .FirstOrDefault(c => c.Id == id && c.ClienteId == cliente.Id);
            if (coleta == null)
            {
                return NotFound();
            }

            var coletaDto = _mapper.Map<ReadColetaDto>(coleta);
            return Ok(coletaDto);
        }

        [HttpPut("me/coletas/{id}")]
        [Authorize(Policy = "ClientePolicy")]
        [SwaggerOperation(Summary = "Atualiza uma coleta do cliente", Description = "Atualiza uma coleta específica solicitada pelo cliente atualmente autenticado")]
        [SwaggerResponse(200, "Coleta atualizada com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Coleta não encontrada")]
        public IActionResult AtualizaColetaDoCliente(int id, [FromBody] UpdateColetaDto coletaDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var cliente = _context.Clientes.FirstOrDefault(c => c.UserId == userId);
            if (cliente == null)
            {
                return NotFound();
            }

            var coleta = _context.Coletas
                .Include(c => c.ColetaResiduos)
                .FirstOrDefault(c => c.Id == id && c.ClienteId == cliente.Id);
            if (coleta == null)
            {
                return NotFound();
            }

            _mapper.Map(coletaDto, coleta);

            // Limpar os resíduos existentes
            coleta.ColetaResiduos.Clear();

            // Associar os resíduos fornecidos na atualização
            foreach (var residuoId in coletaDto.ResiduoIds)
            {
                var residuo = _context.Residuos.Find(residuoId);
                if (residuo == null)
                {
                    return NotFound($"Residuo com ID {residuoId} não encontrado.");
                }

                _context.Attach(residuo); // Garantir que o Residuo está sendo rastreado
                coleta.ColetaResiduos.Add(new ColetaResiduo { ResiduoId = residuoId, Residuo = residuo });
            }

            _context.SaveChanges();
            return Ok(_mapper.Map<ReadColetaDto>(coleta));
        }

        [HttpDelete("me/coletas/{id}")]
        [Authorize(Policy = "ClientePolicy")]
        [SwaggerOperation(Summary = "Deleta uma coleta do cliente", Description = "Deleta uma coleta específica solicitada pelo cliente atualmente autenticado")]
        [SwaggerResponse(204, "Coleta deletada com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Coleta não encontrada")]
        public IActionResult DeletaColetaDoCliente(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var cliente = _context.Clientes.FirstOrDefault(c => c.UserId == userId);
            if (cliente == null)
            {
                return NotFound();
            }

            var coleta = _context.Coletas.FirstOrDefault(c => c.Id == id && c.ClienteId == cliente.Id);
            if (coleta == null)
            {
                return NotFound();
            }

            _context.Remove(coleta);
            _context.SaveChanges();
            return NoContent();
        }
    }
}

