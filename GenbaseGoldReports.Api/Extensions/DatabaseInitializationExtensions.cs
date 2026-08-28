using GenbaseGoldReports.Infrastructure.Persistence;
using GenbaseGoldReports.Infrastructure.Persistence.Seeding;

namespace GenbaseGoldReports.Api.Extensions;

public static class DatabaseInitializationExtensions
{
    public static async Task SeedMasterDatabaseAsync(this IHost app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var masterContext = services.GetRequiredService<MasterDbContext>();
            var config = services.GetRequiredService<IConfiguration>();

            await MasterDbSeeder.SeedAsync(services, config);

            await ApplicationDbSeeder.ProvisionTenantDatabasesAsync(masterContext, services);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating or seeding the databases.");
            throw;
        }
    }
}