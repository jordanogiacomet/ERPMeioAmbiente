using AutoMapper;
using ERPMeioAmbienteAPI.Data;
using ERPMeioAmbienteAPI.Data.Dtos;
using ERPMeioAmbienteAPI.Models;
using ERPMeioAmbienteAPI.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ERPMeioAmbienteAPI.Tests.UnitTests
{
    public class ResiduoServiceTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly IResiduoService _residuoService;
        private readonly TestERPMeioAmbienteContext _context;

        public ResiduoServiceTests()
        {
            _context = MockContextFactory.CreateContext();
            _mapperMock = new Mock<IMapper>();
            _residuoService = new ResiduoService(_context, _mapperMock.Object);
        }

        private void ClearContext()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        [Fact]
        public async Task AddResiduoAsync_ShouldAddResiduo_WhenValid()
        {
            ClearContext();

            var createResiduoDto = new CreateResiduoDto
            {
                Nome = "Plástico",
                Categoria = "Reciclável",
                Tipo = "Polímero"
            };

            var residuo = new Residuo
            {
                Nome = "Plástico",
                Categoria = "Reciclável",
                Tipo = "Polímero"
            };

            _mapperMock.Setup(m => m.Map<Residuo>(It.IsAny<CreateResiduoDto>())).Returns(residuo);

            var result = await _residuoService.AddResiduoAsync(createResiduoDto);

            Assert.Contains(residuo, _context.Residuos);
            Assert.Equal(residuo, result);
        }

        [Fact]
        public async Task GetAllResiduosAsync_ShouldReturnResiduos_WhenCalled()
        {
            ClearContext();

            var residuos = new List<Residuo>
            {
                new Residuo { Id = 1, Nome = "Plástico", Categoria = "Reciclável", Tipo = "Polímero" },
                new Residuo { Id = 2, Nome = "Metal", Categoria = "Reciclável", Tipo = "Ferroso" }
            };

            await _context.Residuos.AddRangeAsync(residuos);
            await _context.SaveChangesAsync();

            var readResiduoDtos = new List<ReadResiduoDto>
            {
                new ReadResiduoDto { Id = 1, Nome = "Plástico", Categoria = "Reciclável", Tipo = "Polímero" },
                new ReadResiduoDto { Id = 2, Nome = "Metal", Categoria = "Reciclável", Tipo = "Ferroso" }
            };

            _mapperMock.Setup(m => m.Map<List<ReadResiduoDto>>(It.IsAny<List<Residuo>>())).Returns(readResiduoDtos);

            var result = await _residuoService.GetAllResiduosAsync(0, 10);

            Assert.Equal(readResiduoDtos, result);
        }

        [Fact]
        public async Task GetResiduoByIdAsync_ShouldReturnResiduo_WhenFound()
        {
            ClearContext();

            var residuo = new Residuo { Id = 1, Nome = "Plástico", Categoria = "Reciclável", Tipo = "Polímero" };

            await _context.Residuos.AddAsync(residuo);
            await _context.SaveChangesAsync();

            var readResiduoDto = new ReadResiduoDto { Id = 1, Nome = "Plástico", Categoria = "Reciclável", Tipo = "Polímero" };

            _mapperMock.Setup(m => m.Map<ReadResiduoDto>(It.IsAny<Residuo>())).Returns(readResiduoDto);

            var result = await _residuoService.GetResiduoByIdAsync(1);

            Assert.Equal(readResiduoDto, result);
        }

        [Fact]
        public async Task GetResiduoByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            ClearContext();

            var result = await _residuoService.GetResiduoByIdAsync(1);

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateResiduoAsync_ShouldReturnTrue_WhenUpdated()
        {
            ClearContext();

            var residuo = new Residuo { Id = 1, Nome = "Plástico", Categoria = "Reciclável", Tipo = "Polímero" };

            await _context.Residuos.AddAsync(residuo);
            await _context.SaveChangesAsync();

            var updateResiduoDto = new UpdateResiduoDto
            {
                Nome = "Papel",
                Categoria = "Reciclável",
                Tipo = "Celulose"
            };

            _mapperMock.Setup(m => m.Map(updateResiduoDto, residuo)).Callback(() => {
                residuo.Nome = updateResiduoDto.Nome;
                residuo.Categoria = updateResiduoDto.Categoria;
                residuo.Tipo = updateResiduoDto.Tipo;
            });

            var result = await _residuoService.UpdateResiduoAsync(1, updateResiduoDto);

            Assert.True(result);
            Assert.Equal("Papel", residuo.Nome);
            Assert.Equal("Reciclável", residuo.Categoria);
            Assert.Equal("Celulose", residuo.Tipo);
        }

        [Fact]
        public async Task DeleteResiduoAsync_ShouldReturnTrue_WhenDeleted()
        {
            ClearContext();

            var residuo = new Residuo { Id = 1, Nome = "Plástico", Categoria = "Reciclável", Tipo = "Polímero" };

            await _context.Residuos.AddAsync(residuo);
            await _context.SaveChangesAsync();

            var result = await _residuoService.DeleteResiduoAsync(1);

            Assert.True(result);

            // Verificar se o resíduo foi removido do contexto
            var deletedResiduo = await _context.Residuos.FindAsync(1);
            Assert.Null(deletedResiduo);
        }
    }
}
