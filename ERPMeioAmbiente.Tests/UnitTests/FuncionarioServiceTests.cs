using AutoMapper;
using ERPMeioAmbiente.API.Data.Dtos;
using ERPMeioAmbienteAPI.Data;
using ERPMeioAmbienteAPI.Data.Dtos;
using ERPMeioAmbienteAPI.Models;
using ERPMeioAmbienteAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ERPMeioAmbienteAPI.Tests.UnitTests
{
    public class FuncionarioServiceTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly FuncionarioService _funcionarioService;
        private readonly TestERPMeioAmbienteContext _context;
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;

        public FuncionarioServiceTests()
        {
            _context = MockContextFactory.CreateContext();
            _mapperMock = new Mock<IMapper>();
            _userManagerMock = MockUserManager();
            _funcionarioService = new FuncionarioService(_context, _mapperMock.Object, _userManagerMock.Object);
        }

        private void ClearContext()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        private Mock<UserManager<IdentityUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<IdentityUser>>();
            return new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
        }

        [Fact]
        public async Task AddFuncionarioAsync_ShouldAddFuncionario_WhenValid()
        {
            ClearContext();

            var createFuncionarioDto = new CreateFuncionarioDto
            {
                Nome = "Teste Funcionario",
                Email = "funcionario@teste.com",
                Password = "Teste@123",
                Telefone = "123456789"
            };

            var identityUser = new IdentityUser
            {
                Email = "funcionario@teste.com",
                UserName = "funcionario@teste.com"
            };

            var funcionario = new Funcionario
            {
                Nome = "Teste Funcionario",
                Email = "funcionario@teste.com",
                UserId = "user-id",
                Telefone = "123456789"
            };

            _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(u => u.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _mapperMock.Setup(m => m.Map<Funcionario>(It.IsAny<CreateFuncionarioDto>())).Returns(funcionario);

            var result = await _funcionarioService.AddFuncionarioAsync(createFuncionarioDto);

            Assert.Contains(funcionario, _context.Funcionarios);
            Assert.Equal(funcionario, result);
        }

        [Fact]
        public async Task GetAllFuncionariosAsync_ShouldReturnFuncionarios_WhenCalled()
        {
            ClearContext();

            var funcionarios = new List<Funcionario>
            {
                new Funcionario { Id = 1, Nome = "Teste Funcionario 1", Email = "funcionario1@teste.com", Telefone = "123456789", UserId = "user-id-1" },
                new Funcionario { Id = 2, Nome = "Teste Funcionario 2", Email = "funcionario2@teste.com", Telefone = "987654321", UserId = "user-id-2" }
            };

            await _context.Funcionarios.AddRangeAsync(funcionarios);
            await _context.SaveChangesAsync();

            var readFuncionarioDtos = new List<ReadFuncionarioDto>
            {
                new ReadFuncionarioDto { Nome = "Teste Funcionario 1", Email = "funcionario1@teste.com", Telefone = "123456789" },
                new ReadFuncionarioDto { Nome = "Teste Funcionario 2", Email = "funcionario2@teste.com", Telefone = "987654321" }
            };

            _mapperMock.Setup(m => m.Map<List<ReadFuncionarioDto>>(It.IsAny<List<Funcionario>>())).Returns(readFuncionarioDtos);

            var result = await _funcionarioService.GetAllFuncionariosAsync(0, 10);

            Assert.Equal(readFuncionarioDtos, result);
        }

        [Fact]
        public async Task GetFuncionarioByIdAsync_ShouldReturnFuncionario_WhenFound()
        {
            // Limpar o contexto
            ClearContext();

            // Dados do funcionário para o teste
            var funcionario = new Funcionario { Id = 1, Nome = "Teste Funcionario", Email = "funcionario@teste.com", Telefone = "123456789", UserId = "user1" };

            // Adicionar o funcionário ao contexto
            await _context.Funcionarios.AddAsync(funcionario);
            await _context.SaveChangesAsync();

            // Verificar se o funcionário foi adicionado corretamente
            var funcionarioNoBanco = await _context.Funcionarios.FirstOrDefaultAsync(f => f.Id == 1);
            Assert.NotNull(funcionarioNoBanco);

            // DTO esperado
            var expectedDto = new ReadFuncionarioDto { Nome = "Teste Funcionario", Email = "funcionario@teste.com", Telefone = "123456789" };

            // Configuração do Mock para o mapeamento
            _mapperMock.Setup(m => m.Map<ReadFuncionarioDto>(It.Is<Funcionario>(f => f.Id == funcionario.Id)))
                       .Returns(expectedDto);

            // Chamar o método do serviço
            var result = await _funcionarioService.GetFuncionarioByIdAsync(1);

            // Verificar se o resultado é igual ao esperado
            Assert.Equal(expectedDto, result);
        }


        [Fact]
        public async Task GetFuncionarioByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            ClearContext();

            var result = await _funcionarioService.GetFuncionarioByIdAsync(1);

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateFuncionarioAsync_ShouldReturnTrue_WhenUpdated()
        {
            ClearContext();

            var funcionario = new Funcionario { Id = 1, Nome = "Teste Funcionario", Email = "funcionario@teste.com", Telefone = "123456789", UserId = "user1" };

            // Adicionar o funcionário ao contexto
            await _context.Funcionarios.AddAsync(funcionario);
            await _context.SaveChangesAsync();

            // Verificar se o funcionário foi adicionado corretamente
            var funcionarioNoBanco = await _context.Funcionarios.FirstOrDefaultAsync(f => f.Id == 1);
            Assert.NotNull(funcionarioNoBanco);

            var updateFuncionarioDto = new UpdateFuncionarioDto
            {
                Nome = "Funcionario Atualizado",
                Email = "atualizado@teste.com",
                Telefone = "987654321"
            };

            // Configuração do Mock para o mapeamento
            _mapperMock.Setup(m => m.Map(updateFuncionarioDto, funcionario)).Callback(() =>
            {
                funcionario.Nome = updateFuncionarioDto.Nome;
                funcionario.Email = updateFuncionarioDto.Email;
                funcionario.Telefone = updateFuncionarioDto.Telefone;
            });

            var result = await _funcionarioService.UpdateFuncionarioAsync(1, updateFuncionarioDto);

            Assert.True(result);

            var updatedFuncionario = await _context.Funcionarios.FindAsync(1);

            Assert.Equal("Funcionario Atualizado", updatedFuncionario.Nome);
            Assert.Equal("atualizado@teste.com", updatedFuncionario.Email);
            Assert.Equal("987654321", updatedFuncionario.Telefone);
        }


        [Fact]
        public async Task DeleteFuncionarioAsync_ShouldReturnTrue_WhenDeleted()
        {
            ClearContext();

            var funcionario = new Funcionario { Id = 1, Nome = "Teste Funcionario", Email = "funcionario@teste.com", Telefone = "123456789", UserId = "user-id" };

            await _context.Funcionarios.AddAsync(funcionario);
            await _context.SaveChangesAsync();

            var result = await _funcionarioService.DeleteFuncionarioAsync(1);

            Assert.True(result);
            Assert.DoesNotContain(funcionario, _context.Funcionarios);
        }
    }
}
