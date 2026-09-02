using GenbaseGoldReports.Application.Common.Interfaces;
using GenbaseGoldReports.Application.Common.Models;
using GenbaseGoldReports.Application.Features.Tenants.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GenbaseGoldReports.Application.Features.Tenants.Queries.GetTenantsPaged
{
    public class GetTenantsPagedQueryHandler : IRequestHandler<GetTenantsPagedQuery, PagedResult<TenantDto>>
    {
        private readonly IMasterDbContext _masterDb;

        public GetTenantsPagedQueryHandler(IMasterDbContext masterDb)
        {
            _masterDb = masterDb;
        }

        public async Task<PagedResult<TenantDto>> Handle(GetTenantsPagedQuery request, CancellationToken cancellationToken)
        {
            var query = _masterDb.Tenants.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var search = request.SearchTerm.Trim().ToLower();
                query = query.Where(t => t.Name.ToLower().Contains(search) || t.Identifier.ToLower().Contains(search));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var tenants = await query
                .OrderByDescending(t => t.CreatedAtUtc)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(t => new TenantDto(
                    t.Id,
                    t.Name,
                    t.Identifier,
                    t.IsActive,
                    t.SubscriptionPlan,
                    t.SubscriptionExpiresAtUtc,
                    t.CreatedAtUtc,
                    t.UpdatedAtUtc
                ))
                .ToListAsync(cancellationToken);

            return new PagedResult<TenantDto>(
                Items: tenants,
                TotalCount: totalCount,
                PageNumber: request.PageNumber,
                PageSize: request.PageSize
            );
        }
    }
}
