using ERPMeioAmbienteAPI.Data;
using ERPMeioAmbienteAPI.Models;
using ERPMeioAmbienteAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;
using System.Text;
using Microsoft.OpenApi.Models;
using ERPMeioAmbiente.API.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("MeioAmbienteConnection");
var key = builder.Configuration["AuthSettings:Key"];
var issuer = builder.Configuration["AuthSettings:Issuer"];
var audience = builder.Configuration["AuthSettings:Audience"];

// Add services to the container.
builder.Services.AddDbContext<ERPMeioAmbienteContext>(opts =>
    opts.UseLazyLoadingProxies().UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredLength = 5;
    options.SignIn.RequireConfirmedEmail = true;
}).AddEntityFrameworkStores<ERPMeioAmbienteContext>()
  .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        RequireExpirationTime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    options.AddPolicy("ClientePolicy", policy => policy.RequireRole("Cliente"));
    options.AddPolicy("FuncionarioPolicy", policy => policy.RequireRole("Funcionario"));
});

builder.Services.AddTransient<IEmailService, EmailServices>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IResiduoService, ResiduoService>();
builder.Services.AddScoped<IFuncionarioService, FuncionarioService>();
builder.Services.AddScoped<IColetaService, ColetaService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IAgendamentoService, AgendamentoService>();
builder.Services.AddScoped<IMotoristaService, MotoristaService>();
builder.Services.AddScoped<IVeiculoService,  VeiculoService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ERPMeioAmbienteAPI", Version = "v1" });
    c.EnableAnnotations();
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Por favor, insira o token JWT no formato 'Bearer {token}'",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManeger = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var context = scope.ServiceProvider.GetRequiredService<ERPMeioAmbienteContext>();

    string[] roles = new[] { "Admin", "Cliente", "Funcionario" };
    foreach (var role in roles)
    {
        if(!await roleManager.RoleExistsAsync(role))
        {
            var roleResult = await roleManager.CreateAsync(new IdentityRole(role));
            
            if(roleResult.Succeeded)
            {
                logger.LogInformation($"Role '{role}' criada com sucesso.");
            }
            else
            {
                logger.LogError($"Erro ao criar role '{role}': {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
            }
        }
    }

    var adminEmail = "admin@example.com";
    var adminUser = await userManeger.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
        };
        var result = await userManeger.CreateAsync(adminUser, "Admin@123");
        if (result.Succeeded)
        {
            logger.LogInformation($"Usuário admin criado com sucesso: {adminEmail}");
            var rolesResult = await userManeger.AddToRolesAsync(adminUser, new[] { "Admin", "Funcionario" });
            if (rolesResult.Succeeded)
            {
                logger.LogInformation($"Roles 'Admin' e 'Funcionario' atribuídas ao usuário {adminEmail} com sucesso.");
            }
            else
            {
                logger.LogError($"Erro ao atribuir roles ao usuário {adminEmail}: {string.Join(", ", rolesResult.Errors.Select(e => e.Description))}");
            }
            var funcionario = new Funcionario
            {
                Nome = "Administrador",
                Email = adminEmail,
                Telefone = "000000000",
                UserId = adminUser.Id,
                User = adminUser
            };
            context.Funcionarios.Add(funcionario);
            await context.SaveChangesAsync();
            logger.LogInformation($"Registro de funcionário criado com sucesso para o usuário {adminEmail}.");
        }
        else
        {
            logger.LogError($"Erro ao criar usuário admin: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
    else
    {
        logger.LogInformation($"Usuário admin já existe: {adminEmail}");
    }


}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
