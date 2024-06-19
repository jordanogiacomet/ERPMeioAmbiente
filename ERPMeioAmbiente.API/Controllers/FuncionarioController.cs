using AutoMapper;
using ERPMeioAmbiente.API.Data.Dtos;
using ERPMeioAmbienteAPI.Data;
using ERPMeioAmbienteAPI.Data.Dtos;
using ERPMeioAmbienteAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ERPMeioAmbiente.API.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class FuncionarioController : ControllerBase
    {
        private ERPMeioAmbienteContext _context;
        private AutoMapper.IMapper _mapper;
        
        public FuncionarioController(ERPMeioAmbienteContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpPost]
        public IActionResult AdicionaFuncionario([FromBody] CreateFuncionarioDto funcionarioDto)
        {
            Funcionario funcionario = _mapper.Map<Funcionario>(funcionarioDto);
            _context.Funcionarios.Add(funcionario);
            _context.SaveChanges();
            return CreatedAtAction(nameof(RecuperaFuncionarioPorId), new { id = funcionario.Id }, funcionario);
        }
        [HttpGet]
        public IEnumerable<ReadFuncionarioDto> RecuperaFuncionarios([FromQuery] int skip = 0, [FromQuery] int take = 50)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier);
            var rng = new Random();
            return _mapper.Map<List<ReadFuncionarioDto>>(_context.Funcionarios.Skip(skip).Take(take));
        }

        [HttpGet("{id}")]
        public IActionResult RecuperaFuncionarioPorId(int id)
        {
            var funcionario = _context.Funcionarios.FirstOrDefault(funcionario => funcionario.Id == id);
            if (funcionario == null)
            {
                return NotFound();
            }
            var funcionarioDto = _mapper.Map<ReadClienteDto>(funcionario);
            return Ok(funcionarioDto);
        }

        [HttpPut("{id}")]
        public IActionResult AtualizaFuncionario(int id, [FromBody] UpdateFuncionarioDto funcionarioDto)
        {
            var funcionario = _context.Funcionarios.FirstOrDefault(funcionario => funcionario.Id == id);
            if (funcionario == null) return NotFound();
            _mapper.Map(funcionarioDto, funcionario);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeletaFuncionario(int id)
        {
            var funcionario = _context.Funcionarios.FirstOrDefault(funcionario => funcionario.Id == id);
            if (funcionario == null) return NotFound();
            _context.Remove(funcionario);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
