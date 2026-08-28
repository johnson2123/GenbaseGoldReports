using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace GenbaseGoldReports.Infrastructure.Persistence.Factories;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../GenbaseGoldReports.Api");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var masterConn = configuration.GetConnectionString("MasterConnection")
            ?? "Server=.;Database=GenbaseGoldMaster;Trusted_Connection=True;TrustServerCertificate=True;";

        // Build a design-time template connection string on the same configured server
        var builder = new SqlConnectionStringBuilder(masterConn)
        {
            InitialCatalog = "GenbaseGoldReports_DesignTemplate"
        };

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer(builder.ConnectionString);

        return new ApplicationDbContext(optionsBuilder.Options, null!);
    }
}