using ERPMeioAmbienteAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ERPMeioAmbienteAPI.Data
{
    public class ERPMeioAmbienteContext : IdentityDbContext
    {
        public ERPMeioAmbienteContext(DbContextOptions<ERPMeioAmbienteContext> opts) 
            : base(opts)
        {
            
        }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }
    }
}
