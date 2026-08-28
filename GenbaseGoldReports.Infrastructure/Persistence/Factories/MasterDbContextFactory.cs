using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace GenbaseGoldReports.Infrastructure.Persistence.Factories;

public class MasterDbContextFactory : IDesignTimeDbContextFactory<MasterDbContext>
{
    public MasterDbContext CreateDbContext(string[] args)
    {
        // Dynamically build configuration to read appsettings.json from the API project folder
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../GenbaseGoldReports.Api");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString("MasterConnection")
            ?? "Server=.;Database=GenbaseGoldMaster;Trusted_Connection=True;TrustServerCertificate=True;";

        var optionsBuilder = new DbContextOptionsBuilder<MasterDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new MasterDbContext(optionsBuilder.Options);
    }
}