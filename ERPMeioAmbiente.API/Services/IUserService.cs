using ERPMeioAmbiente.Shared;
using ERPMeioAmbienteAPI.Data;
using ERPMeioAmbienteAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ERPMeioAmbienteAPI.Services
{
    public interface IUserService
    {
        Task<UserManegerResponse> RegisterUserAsync(RegisterViewModel model);
        Task<UserManegerResponse> LoginUserAsync(LoginViewModel model);
    }

    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ERPMeioAmbienteContext _context;

        public UserService(UserManager<IdentityUser> userManager, IConfiguration configuration, ERPMeioAmbienteContext context)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
        }

        public async Task<UserManegerResponse> RegisterUserAsync(RegisterViewModel model)
        {
            if (model == null)
            {
                throw new NullReferenceException("Register Model is null");
            }

            if (model.Password != model.ConfirmPassword)
            {
                return new UserManegerResponse
                {
                    Message = "Confirm password doesn't match the password",
                    IsSuccess = false,
                };
            }

            var identityUser = new IdentityUser
            {
                Email = model.Email,
                UserName = model.Email,
            };

            var result = await _userManager.CreateAsync(identityUser, model.Password);

            if (result.Succeeded)
            {

                var cliente = new Cliente
                {
                    Nome = model.Nome,
                    Contato = model.Contato,
                    CNPJ = model.CNPJ,
                    Endereco = model.Endereco,
                    CEP = model.CEP,
                    UserId = identityUser.Id,
                };

                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();

                return new UserManegerResponse
                {
                    Message = "User created successfully",
                    IsSuccess = true,
                };
            }

            return new UserManegerResponse
            {
                Message = "User did not create",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<UserManegerResponse> LoginUserAsync(LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return new UserManegerResponse
                {
                    Message = "There is no user with that Email address",
                    IsSuccess = false,
                };
            }

            var result = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!result)
            {
                return new UserManegerResponse
                {
                    Message = "Invalid password",
                    IsSuccess = false,
                };
            }

            var claims = new[]
            {
                new Claim("Email", model.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["AuthSettings:Issuer"],
                audience: _configuration["AuthSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            var tokenHandler = new JwtSecurityTokenHandler();

            string tokenAsString = tokenHandler.WriteToken(token);

            return new UserManegerResponse
            {
                Message = tokenAsString,
                IsSuccess = true,
                ExpireDate = token.ValidTo
            };
        }
    }
}
