using AutoMapper;
using ERPMeioAmbienteAPI.Data;
using ERPMeioAmbienteAPI.Data.Dtos;
using ERPMeioAmbienteAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public ClienteController(ERPMeioAmbienteContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        public IActionResult AdicionaCliente([FromBody] CreateClienteDto clienteDto)
        {
            Cliente cliente = _mapper.Map<Cliente>(clienteDto);
            _context.Clientes.Add(cliente);
            _context.SaveChanges();
            return CreatedAtAction(nameof(RecuperaClientePorId), new { id = cliente.Id }, cliente);
        }

        [HttpGet]
        [Authorize(Policy = "FuncionarioPolicy")]
        public IEnumerable<ReadClienteDto> RecuperaClientes([FromQuery] int skip = 0, [FromQuery] int take = 50)
        {
            return _mapper.Map<List<ReadClienteDto>>(_context.Clientes.Skip(skip).Take(take));
        }

        [HttpGet("me")]
        [Authorize(Policy = "ClientePolicy")]
        public IActionResult RecuperaClienteAtual()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                throw new Exception();
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
            return NoContent();
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AdminPolicy, FuncionarioPolicy")]
        public IActionResult RecuperaClientePorId(int id)
        {
            var cliente = _context.Clientes.FirstOrDefault(cliente => cliente.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }

            var clienteDto = _mapper.Map<ReadClienteDto>(cliente);
            return Ok(clienteDto);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminPolicy, FuncionarioPolicy")]
        public IActionResult AtualizaCliente(int id, [FromBody] UpdateClienteDto clienteDto)
        {
            var cliente = _context.Clientes.FirstOrDefault(c => c.Id == id);
            if (cliente == null) return NotFound();

            _mapper.Map(clienteDto, cliente);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public IActionResult DeletaCliente(int id)
        {
            var cliente = _context.Clientes.FirstOrDefault(cliente => cliente.Id == id);
            if (cliente == null) return NotFound();
            _context.Remove(cliente);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
