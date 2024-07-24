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
        public DbSet<Residuo> Residuos { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<Coleta> Coletas { get; set; }
        public DbSet<ColetaResiduo> ColetaResiduos { get; set; }
        public DbSet<Agendamento> Agendamentos { get; set; }
        public DbSet<Motorista> Motoristas { get; set; }
        public DbSet<Veiculo> Veiculos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relação Motorista-Veiculo (1-1)
            modelBuilder.Entity<Motorista>()
                .HasOne(m => m.Veiculo)
                .WithOne(v => v.Motorista)
                .HasForeignKey<Veiculo>(v => v.MotoristaId);

            // Relação Cliente-Coleta (1-N)
            modelBuilder.Entity<Cliente>()
                .HasMany(c => c.Coletas)
                .WithOne(c => c.Cliente)
                .HasForeignKey(c => c.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relação Coleta-Residuo (N-N) com tabela de junção ColetaResiduo
            modelBuilder.Entity<ColetaResiduo>()
                .HasKey(cr => new { cr.ColetaId, cr.ResiduoId });

            modelBuilder.Entity<ColetaResiduo>()
                .HasOne(cr => cr.Coleta)
                .WithMany(c => c.ColetaResiduos)
                .HasForeignKey(cr => cr.ColetaId);

            modelBuilder.Entity<ColetaResiduo>()
                .HasOne(cr => cr.Residuo)
                .WithMany(r => r.ColetaResiduos)
                .HasForeignKey(cr => cr.ResiduoId);

            // Relação Coleta-Agendamento (1-1)
            modelBuilder.Entity<Agendamento>()
                .HasOne(a => a.Coleta)
                .WithOne(c => c.Agendamento)
                .HasForeignKey<Agendamento>(a => a.ColetaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
