namespace GenbaseGoldReports.Application.Common.Interfaces
{
    public interface ITenantProvisioningService
    {
        Task ProvisionTenantDatabaseAsync(string connectionString, CancellationToken cancellationToken = default);
    }
}
