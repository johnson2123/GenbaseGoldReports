using GenbaseGoldReports.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace GenbaseGoldReports.Infrastructure.Persistence.Seeding;

public static class ApplicationDbSeeder
{
    public static async Task ProvisionTenantDatabasesAsync(MasterDbContext masterContext, IServiceProvider serviceProvider)
    {
        var tenants = await masterContext.Tenants
            .Where(t => t.IsActive && t.Identifier != "admin")
            .ToListAsync();

        foreach (var tenant in tenants)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(tenant.ConnectionString);

            var tenantContext = new TenantContext();
            tenantContext.SetTenant(tenant.Id, tenant.Identifier, tenant.ConnectionString);

            using var dbContext = new ApplicationDbContext(optionsBuilder.Options, tenantContext);

            await dbContext.Database.MigrateAsync();
        }
    }
}