namespace GenbaseGoldReports.Application.Common.Interfaces
{
    public interface ITenantContext
    {
        Guid TenantId { get; }
        string Identifier { get; }
        string ConnectionString { get; }
        bool IsResolved { get; }
        void SetTenant(Guid tenantId, string identifier, string connectionString);
    }
}
