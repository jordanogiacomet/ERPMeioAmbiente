using Microsoft.EntityFrameworkCore;
using ERPMeioAmbienteAPI.Data;

public static class MockContextFactory
{
    public static TestERPMeioAmbienteContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ERPMeioAmbienteContext>()
            .UseInMemoryDatabase(databaseName: "ERPMeioAmbienteTestDB")
            .Options;

        var context = new TestERPMeioAmbienteContext(options);
        context.Database.EnsureCreated();

        return context;
    }
}
