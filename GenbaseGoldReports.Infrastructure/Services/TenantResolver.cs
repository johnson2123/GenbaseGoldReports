using GenbaseGoldReports.Application.Common.Interfaces;
using GenbaseGoldReports.Application.Common.Options;
using GenbaseGoldReports.Domain.Entities;
using GenbaseGoldReports.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace GenbaseGoldReports.Infrastructure.Services
{
    public class TenantResolver : ITenantResolver
    {
        private readonly MasterDbContext _masterDbContext;
        private readonly IMemoryCache _cache;
        private readonly TenantSettings _tenantSettings;

        public TenantResolver(MasterDbContext masterDbContext, IMemoryCache cache, IOptions<TenantSettings> tenantSettings)
        {
            _masterDbContext = masterDbContext;
            _cache = cache;
            _tenantSettings = tenantSettings.Value;
        }

        public async Task<Tenant?> ResolveAsync(string identifier, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                return null;

            string cacheKey = $"tenant:{identifier.ToLowerInvariant()}";

            if (_cache.TryGetValue(cacheKey, out Tenant? cachedTenant))
            {
                return cachedTenant;
            }

            var tenant = await _masterDbContext.Tenants
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Identifier == identifier.ToLowerInvariant() && t.IsActive, cancellationToken);

            if (tenant != null)
            {
                var cacheDuration = TimeSpan.FromMinutes(_tenantSettings.CacheDurationInMinutes);
                _cache.Set(cacheKey, tenant, cacheDuration);
            }

            return tenant;
        }
    }
}
