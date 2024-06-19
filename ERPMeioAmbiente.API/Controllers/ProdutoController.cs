using AutoMapper;
using ERPMeioAmbienteAPI.Data;
using ERPMeioAmbienteAPI.Data.Dtos;
using ERPMeioAmbienteAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ERPMeioAmbienteAPI.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class ProdutoController : ControllerBase
    {
        private ERPMeioAmbienteContext _context;
        private AutoMapper.IMapper _mapper;

        public ProdutoController(ERPMeioAmbienteContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpPost]
        public IActionResult AdicionaProduto([FromBody] CreateProdutoDto produtoDto)
        {
            Produto produto = _mapper.Map<Produto>(produtoDto);
            _context.Produtos.Add(produto);
            _context.SaveChanges();
            return CreatedAtAction(nameof(RecuperaProdutoPorId), new { id = produto.Id }, produto);
        }

        [HttpGet]
        public IEnumerable<ReadProdutoDto> RecuperaProdutos([FromQuery] int skip = 0, [FromQuery] int take = 50)
        {
            return _mapper.Map<List<ReadProdutoDto>>(_context.Produtos.Skip(skip).Take(take));
        }

        [HttpGet("{id}")]
        public IActionResult RecuperaProdutoPorId(int id)
        {
            var produto = _context.Produtos.FirstOrDefault(produto => produto.Id == id);
            if (produto == null)
            {
                return NotFound();
            }
            var produtoDto = _mapper.Map<ReadProdutoDto>(produto);
            return Ok(produtoDto);
        }

        [HttpPut("{id}")]
        public IActionResult AtualizaProduto(int id, [FromBody] UpdateProdutoDto produtoDto)
        {
            var produto = _context.Produtos.FirstOrDefault(produto => produto.Id == id);
            if (produto == null) return NotFound();
            _mapper.Map(produtoDto, produto);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeletaProduto(int id)
        {
            var produto = _context.Produtos.FirstOrDefault(produto => produto.Id == id);
            if (produto == null) return NotFound();
            _context.Remove(produto);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
