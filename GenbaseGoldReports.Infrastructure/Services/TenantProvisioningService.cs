using GenbaseGoldReports.Application.Common.Interfaces;
using GenbaseGoldReports.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GenbaseGoldReports.Infrastructure.Services
{
    public class TenantProvisioningService : ITenantProvisioningService
    {
        public async Task ProvisionTenantDatabaseAsync(string connectionString, CancellationToken cancellationToken)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            using var tenantDbContext = new ApplicationDbContext(
                optionsBuilder.Options,
                new TenantContext());

            await tenantDbContext.Database.MigrateAsync(cancellationToken);
        }
    }
}
