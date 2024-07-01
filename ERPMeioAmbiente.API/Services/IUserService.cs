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
        Task<UserManegerResponse> ForgotPasswordAsync(ForgotPasswordViewModel model);
        Task<UserManegerResponse> ResetPasswordAsync(ResetPasswordViewModel model);
        Task<UserManegerResponse> ConfirmEmailAsync(string userId, string token);
    }


    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ERPMeioAmbienteContext _context;
        private readonly IEmailService _emailService;

        public UserService(UserManager<IdentityUser> userManager, IConfiguration configuration, ERPMeioAmbienteContext context, IEmailService emailService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
            _emailService = emailService;
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
                await _userManager.AddToRoleAsync(identityUser, "Cliente");
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

                var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
                var confirmationLink = $"http://localhost:7186/api/Auth/confirm-email?userId={identityUser.Id}&token={confirmationToken}";
                await _emailService.SendEmailAsync(identityUser.Email, "Confirme seu email", $"Por favor, confirme seu email clicando no link: {confirmationLink}");

                return new UserManegerResponse
                {
                    Message = "User created successfully. Please check your email to confirm your account.",
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

            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, model.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

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

        public async Task<UserManegerResponse> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new UserManegerResponse
                {
                    Message = "User not found",
                    IsSuccess = false,
                };
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return new UserManegerResponse
                {
                    Message = "Email confirmed successfully",
                    IsSuccess = true,
                };
            }

            return new UserManegerResponse
            {
                Message = "Email confirmation failed",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description),
            };
        }

        public async Task<UserManegerResponse> ForgotPasswordAsync(ForgotPasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new UserManegerResponse
                {
                    Message = "No user associated with email",
                    IsSuccess = false,
                };
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"https://yourapp.com/resetpassword?email={model.Email}&token={token}";

            // You would typically send the link via email
            // For simplicity, we are returning the link in the response
            return new UserManegerResponse
            {
                Message = resetLink,
                IsSuccess = true,
            };
        }

        public async Task<UserManegerResponse> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new UserManegerResponse
                {
                    Message = "No user associated with email",
                    IsSuccess = false,
                };
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                return new UserManegerResponse
                {
                    Message = "Password and Confirm Password do not match",
                    IsSuccess = false,
                };
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded)
            {
                return new UserManegerResponse
                {
                    Message = "Password has been reset successfully",
                    IsSuccess = true,
                };
            }

            return new UserManegerResponse
            {
                Message = "Error while resetting the password",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description),
            };
        }
    }
}
