using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using ERPMeioAmbienteAPI.Services;
using ERPMeioAmbienteAPI.Data;
using ERPMeioAmbienteAPI.Models;
using ERPMeioAmbiente.Shared;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ERPMeioAmbienteAPI.Tests.UnitTests
{
    public class UserServiceTests
    {
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly ERPMeioAmbienteContext _context;
        private readonly IUserService _userService;
        private readonly Mock<IEmailService> _emailServiceMock;

        public UserServiceTests()
        {
            _userManagerMock = MockUserManager.CreateMockUserManager<IdentityUser>();
            _configurationMock = new Mock<IConfiguration>();
            _emailServiceMock = new Mock<IEmailService>();

            // Configurando o contexto de banco de dados em memória
            var options = new DbContextOptionsBuilder<ERPMeioAmbienteContext>()
                .UseInMemoryDatabase(databaseName: "ERPMeioAmbienteTestDB")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _context = new ERPMeioAmbienteContext(options);

            _userService = new UserService(
                _userManagerMock.Object,
                _configurationMock.Object,
                _context,
                _emailServiceMock.Object);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnSuccess_WhenPasswordsMatch()
        {
            var model = new RegisterViewModel
            {
                Email = "test@example.com",
                Password = "Test@123",
                ConfirmPassword = "Test@123",
                Nome = "Test User",
                Contato = "123456789",
                CNPJ = "12345678912345",
                Endereco = "Test Address",
                CEP = "12345-678"
            };

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), "Cliente"))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _userService.RegisterUserAsync(model);

            Assert.True(result.IsSuccess);
            Assert.Equal("User created successfully.", result.Message);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnFailure_WhenPasswordsDoNotMatch()
        {
            var model = new RegisterViewModel
            {
                Email = "test@example.com",
                Password = "Test@123",
                ConfirmPassword = "Test@124"
            };

            var result = await _userService.RegisterUserAsync(model);

            Assert.False(result.IsSuccess);
            Assert.Equal("Confirm password doesn't match the password", result.Message);
        }

        [Fact]
        public async Task LoginUserAsync_ShouldReturnSuccess_WhenCredentialsAreValid()
        {
            var model = new LoginViewModel
            {
                Email = "test@example.com",
                Password = "Test@123"
            };

            var user = new IdentityUser { Email = model.Email, UserName = model.Email };

            _userManagerMock.Setup(x => x.FindByEmailAsync(model.Email))
                .ReturnsAsync(user);

            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, model.Password))
                .ReturnsAsync(true);

            _userManagerMock.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new[] { "Cliente" });

            _configurationMock.Setup(x => x["AuthSettings:Key"])
                .Returns("This is the key that will be use in the encryption");
            _configurationMock.Setup(x => x["AuthSettings:Issuer"])
                .Returns("TestIssuer");
            _configurationMock.Setup(x => x["AuthSettings:Audience"])
                .Returns("TestAudience");

            var result = await _userService.LoginUserAsync(model);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Message);
        }

        [Fact]
        public async Task ConfirmEmailAsync_ShouldReturnSuccess_WhenTokenIsValid()
        {
            var userId = "testUserId";
            var token = "testToken";

            var user = new IdentityUser { Id = userId };

            _userManagerMock.Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _userManagerMock.Setup(x => x.ConfirmEmailAsync(user, token))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _userService.ConfirmEmailAsync(userId, token);

            Assert.True(result.IsSuccess);
            Assert.Equal("Email confirmed successfully", result.Message);
        }

        [Fact]
        public async Task ForgotPasswordAsync_ShouldReturnSuccess_WhenUserExists()
        {
            var model = new ForgotPasswordViewModel { Email = "test@example.com" };
            var user = new IdentityUser { Email = model.Email };

            _userManagerMock.Setup(x => x.FindByEmailAsync(model.Email))
                .ReturnsAsync(user);

            _userManagerMock.Setup(x => x.GeneratePasswordResetTokenAsync(user))
                .ReturnsAsync("resetToken");

            var result = await _userService.ForgotPasswordAsync(model);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Message);
        }

        [Fact]
        public async Task ResetPasswordAsync_ShouldReturnSuccess_WhenResetIsSuccessful()
        {
            var model = new ResetPasswordViewModel
            {
                Email = "test@example.com",
                Token = "resetToken",
                NewPassword = "NewTest@123",
                ConfirmPassword = "NewTest@123"
            };
            var user = new IdentityUser { Email = model.Email };

            _userManagerMock.Setup(x => x.FindByEmailAsync(model.Email))
                .ReturnsAsync(user);

            _userManagerMock.Setup(x => x.ResetPasswordAsync(user, model.Token, model.NewPassword))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _userService.ResetPasswordAsync(model);

            Assert.True(result.IsSuccess);
            Assert.Equal("Password has been reset successfully", result.Message);
        }
    }
}
