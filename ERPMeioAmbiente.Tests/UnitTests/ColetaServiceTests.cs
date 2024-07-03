using AutoMapper;
using ERPMeioAmbiente.API.Data.Dtos;
using ERPMeioAmbiente.API.Models;
using ERPMeioAmbienteAPI.Data;
using ERPMeioAmbienteAPI.Data.Dtos;
using ERPMeioAmbienteAPI.Models;
using ERPMeioAmbienteAPI.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ERPMeioAmbienteAPI.Tests.UnitTests
{
    public class ColetaServiceTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly IColetaService _coletaService;
        private readonly TestERPMeioAmbienteContext _context;
        public ColetaServiceTests()
        {
            _context = MockContextFactory.CreateContext();
            _mapperMock = new Mock<IMapper>();
            _coletaService = new ColetaService(_context, _mapperMock.Object);
        }

        private void ClearContext()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        [Fact]
        public async Task AddColetaAsync_ShouldAddColeta_WhenValid()
        {
            ClearContext();

            var createColetaDto = new CreateColetaDto
            {
                Nvol = "123",
                Peso = "10kg",
                Dimensoes = "1x1x1",
                Endereco = "Rua Teste, 123",
                ClienteId = 1,
                ResiduoIds = new List<int> { 1 }
            };

            var coleta = new Coleta
            {
                Nvol = "123",
                Peso = "10kg",
                Dimensoes = "1x1x1",
                Endereco = "Rua Teste, 123",
                ClienteId = 1,
                ColetaResiduos = new List<ColetaResiduo>
        {
            new ColetaResiduo { ResiduoId = 1 }
        }
            };

            var residuo = new Residuo { Id = 1, Nome = "Plástico", Categoria = "Reciclável", Tipo = "Polímero" };

            await _context.Residuos.AddAsync(residuo);
            await _context.SaveChangesAsync();

            _mapperMock.Setup(m => m.Map<Coleta>(It.IsAny<CreateColetaDto>())).Returns(coleta);

            var result = await _coletaService.AddColetaAsync(createColetaDto);

            Assert.Contains(coleta, _context.Coletas);
            Assert.Equal(coleta, result);
            Assert.Contains(coleta.ColetaResiduos, cr => cr.ResiduoId == residuo.Id);
        }

        [Fact]
        public async Task GetAllColetasAsync_ShouldReturnColetas_WhenCalled()
        {
            ClearContext();

            var coletas = new List<Coleta>
            {
                new Coleta { Id = 1, Nvol = "123", Peso = "10kg", Dimensoes = "1x1x1", Endereco = "Rua Teste, 123", ClienteId = 1 },
                new Coleta { Id = 2, Nvol = "124", Peso = "20kg", Dimensoes = "2x2x2", Endereco = "Rua Teste, 124", ClienteId = 2 }
            };

            await _context.Coletas.AddRangeAsync(coletas);
            await _context.SaveChangesAsync();

            var readColetaDtos = new List<ReadColetaDto>
            {
                new ReadColetaDto { Id = 1, Nvol = "123", Peso = "10kg", Dimensoes = "1x1x1", Endereco = "Rua Teste, 123" },
                new ReadColetaDto { Id = 2, Nvol = "124", Peso = "20kg", Dimensoes = "2x2x2", Endereco = "Rua Teste, 124" }
            };

            _mapperMock.Setup(m => m.Map<List<ReadColetaDto>>(It.IsAny<List<Coleta>>())).Returns(readColetaDtos);

            var result = await _coletaService.GetAllColetasAsync(0, 10);

            Assert.Equal(readColetaDtos, result);
        }

        [Fact]
        public async Task GetColetaByIdAsync_ShouldReturnColeta_WhenFound()
        {
            ClearContext();

            var coleta = new Coleta { Id = 1, Nvol = "123", Peso = "10kg", Dimensoes = "1x1x1", Endereco = "Rua Teste, 123", ClienteId = 1 };

            await _context.Coletas.AddAsync(coleta);
            await _context.SaveChangesAsync();

            var readColetaDto = new ReadColetaDto { Id = 1, Nvol = "123", Peso = "10kg", Dimensoes = "1x1x1", Endereco = "Rua Teste, 123" };

            _mapperMock.Setup(m => m.Map<ReadColetaDto>(It.IsAny<Coleta>())).Returns(readColetaDto);

            var result = await _coletaService.GetColetaByIdAsync(1);

            Assert.Equal(readColetaDto, result);
        }

        [Fact]
        public async Task GetColetaByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            ClearContext();

            var result = await _coletaService.GetColetaByIdAsync(1);

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateColetaAsync_ShouldReturnTrue_WhenUpdated()
        {
            ClearContext();

            var coleta = new Coleta { Id = 1, Nvol = "123", Peso = "10kg", Dimensoes = "1x1x1", Endereco = "Rua Teste, 123", ClienteId = 1 };

            await _context.Coletas.AddAsync(coleta);
            await _context.SaveChangesAsync();

            var updateColetaDto = new UpdateColetaDto
            {
                Nvol = "124",
                Peso = "20kg",
                Dimensoes = "2x2x2",
                Endereco = "Rua Teste, 124",
                ResiduoIds = new List<int> { 1 }
            };

            var residuo = new Residuo { Id = 1, Nome = "Plástico", Categoria = "Reciclável", Tipo = "Polímero" };

            await _context.Residuos.AddAsync(residuo);
            await _context.SaveChangesAsync();

            _mapperMock.Setup(m => m.Map(updateColetaDto, coleta)).Callback(() =>
            {
                coleta.Nvol = updateColetaDto.Nvol;
                coleta.Peso = updateColetaDto.Peso;
                coleta.Dimensoes = updateColetaDto.Dimensoes;
                coleta.Endereco = updateColetaDto.Endereco;
            });

            var result = await _coletaService.UpdateColetaAsync(1, updateColetaDto);

            Assert.True(result);
            Assert.Equal("124", coleta.Nvol);
            Assert.Equal("20kg", coleta.Peso);
            Assert.Equal("2x2x2", coleta.Dimensoes);
            Assert.Equal("Rua Teste, 124", coleta.Endereco);
            Assert.Contains(coleta.ColetaResiduos, cr => cr.ResiduoId == residuo.Id);
        }

        [Fact]
        public async Task DeleteColetaAsync_ShouldReturnTrue_WhenDeleted()
        {
            ClearContext();

            var coleta = new Coleta
            {
                Id = 1,
                Nvol = "123",
                Peso = "10kg",
                Dimensoes = "1x1x1",
                Endereco = "Rua Teste, 123",
                ClienteId = 1
            };

            // Adiciona a coleta ao contexto
            await _context.Coletas.AddAsync(coleta);
            await _context.SaveChangesAsync();

            // Verifica se a coleta foi adicionada corretamente
            var coletaNoBanco = await _context.Coletas.FirstOrDefaultAsync(c => c.Id == 1);
            Assert.NotNull(coletaNoBanco);

            // Tenta deletar a coleta
            var result = await _coletaService.DeleteColetaAsync(1);

            // Verifica se a coleta foi deletada corretamente
            Assert.True(result);

            var deletedColeta = await _context.Coletas.FindAsync(1);
            Assert.Null(deletedColeta);
        }
    }
}