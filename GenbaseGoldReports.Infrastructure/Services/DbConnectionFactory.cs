using GenbaseGoldReports.Application.Common.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace GenbaseGoldReports.Infrastructure.Services
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly ITenantContext _tenantContext;

        public DbConnectionFactory(ITenantContext tenantContext)
        {
            _tenantContext = tenantContext;
        }

        public IDbConnection CreateConnection()
        {
            if (!_tenantContext.IsResolved || string.IsNullOrEmpty(_tenantContext.ConnectionString))
            {
                throw new InvalidOperationException("Cannot create DB connection because Tenant Context has not been resolved.");
            }

            return new SqlConnection(_tenantContext.ConnectionString);
        }
    }
}
