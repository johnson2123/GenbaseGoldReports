using GenbaseGoldReports.Domain.Entities;

namespace GenbaseGoldReports.Application.Common.Interfaces
{
    public interface ITenantResolver
    {
        Task<Tenant?> ResolveAsync(string identifier, CancellationToken cancellationToken = default);
    }
}
