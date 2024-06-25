using ERPMeioAmbiente.API.Models;
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
        public DbSet<Coleta> Coletas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cliente>()
                .HasMany(c => c.Coletas)
                .WithOne(c => c.Cliente)
                .HasForeignKey(c => c.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
