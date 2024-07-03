using Microsoft.EntityFrameworkCore;
using ERPMeioAmbienteAPI.Data;

public class TestERPMeioAmbienteContext : ERPMeioAmbienteContext
{
    public TestERPMeioAmbienteContext(DbContextOptions<ERPMeioAmbienteContext> options)
        : base(options)
    {
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Configurações adicionais, se necessário
    }
}