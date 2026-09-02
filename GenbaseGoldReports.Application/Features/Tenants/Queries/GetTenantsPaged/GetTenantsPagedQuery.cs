using GenbaseGoldReports.Application.Common.Models;
using GenbaseGoldReports.Application.Features.Tenants.Dtos;
using MediatR;

namespace GenbaseGoldReports.Application.Features.Tenants.Queries.GetTenantsPaged
{
    public record GetTenantsPagedQuery : PagedRequest, IRequest<PagedResult<TenantDto>>;
}
