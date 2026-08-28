using GenbaseGoldReports.Application.Common.Interfaces;

namespace GenbaseGoldReports.Infrastructure.Services
{
    public class TenantContext : ITenantContext
    {
        public Guid TenantId { get; private set; }
        public string Identifier { get; private set; } = string.Empty;
        public string ConnectionString { get; private set; } = string.Empty;
        public bool IsResolved { get; private set; }

        public void SetTenant(Guid tenantId, string identifier, string connectionString)
        {
            if (IsResolved)
            {
                throw new InvalidOperationException("Tenant context has already been initialized for this request.");
            }

            TenantId = tenantId;
            Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            IsResolved = true;
        }
    }
}
