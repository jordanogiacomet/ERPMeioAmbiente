using AutoMapper;
using ERPMeioAmbienteAPI.Data;
using ERPMeioAmbienteAPI.Data.Dtos;
using ERPMeioAmbienteAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace ERPMeioAmbienteAPI.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize]
    public class ClienteController : ControllerBase
    {
        private ERPMeioAmbienteContext _context;
        private AutoMapper.IMapper _mapper;
        public ClienteController(ERPMeioAmbienteContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpPost]
        public IActionResult AdicionaCliente([FromBody] CreateClienteDto clienteDto)
        {
            Cliente cliente = _mapper.Map<Cliente>(clienteDto);
            _context.Clientes.Add(cliente);
            _context.SaveChanges();
            return CreatedAtAction(nameof(RecuperaClientePorId), new { id = cliente.Id }, cliente);
        }

        [HttpGet]
        public IEnumerable<ReadClienteDto> RecuperaClientes([FromQuery] int skip = 0, [FromQuery] int take = 50)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier);
            var rng = new Random();
            return _mapper.Map<List<ReadClienteDto>>(_context.Clientes.Skip(skip).Take(take));
        }

        [HttpGet("{id}")]
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
        public IActionResult AtualizaCliente(int id, [FromBody] UpdateClienteDto clienteDto)
        {
            var cliente = _context.Clientes.FirstOrDefault(cliente => cliente.Id == id);
            if (cliente == null) return NotFound();
            _mapper.Map(clienteDto, cliente);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
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
