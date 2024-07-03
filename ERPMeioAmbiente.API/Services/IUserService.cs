using ERPMeioAmbiente.Shared;
using ERPMeioAmbienteAPI.Data;
using ERPMeioAmbienteAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;

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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(UserManager<IdentityUser> userManager, IConfiguration configuration, ERPMeioAmbienteContext context, IEmailService emailService, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
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

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
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

                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
                        var linkGenerator = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<LinkGenerator>();
                        var confirmationLink = linkGenerator.GetUriByAction(
                            _httpContextAccessor.HttpContext,
                            action: "ConfirmEmail",
                            controller: "User",
                            values: new { token, email = identityUser.Email });

                        var subject = "Welcome to ERPMeioAmbiente";
                        var body = $"Hello {model.Nome},<br><br>Thank you for registering at ERPMeioAmbiente. Please confirm your email by clicking <a href='{confirmationLink}'>here</a>.";

                        await _emailService.SendEmailAsync(model.Email, subject, body);

                        await transaction.CommitAsync();

                        return new UserManegerResponse
                        {
                            Message = "User created successfully.",
                            IsSuccess = true,
                        };
                    }

                    await transaction.RollbackAsync();
                    return new UserManegerResponse
                    {
                        Message = "User did not create",
                        IsSuccess = false,
                        Errors = result.Errors.Select(e => e.Description)
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new UserManegerResponse
                    {
                        Message = $"An error occurred: {ex.Message}",
                        IsSuccess = false
                    };
                }
            }
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
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return new UserManegerResponse
                {
                    Message = "User ID or token is invalid",
                    IsSuccess = false,
                };
            }

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
                Errors = result.Errors.Select(e => e.Description).ToList(),
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

            return new UserManegerResponse
            {
                Message = token,
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
