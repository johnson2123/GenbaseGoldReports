using GenbaseGoldReports.Application.Features.Tenants.Dtos;
using GenbaseGoldReports.Domain.Entities;
using MediatR;

namespace GenbaseGoldReports.Application.Features.Tenants.Commands.UpdateTenantSubscription
{
    public record UpdateTenantSubscriptionCommand(
        Guid TenantId,
        TenantSubscriptionPlan SubscriptionPlan,
        DateTime? CustomExpirationDateUtc = null
    ) : IRequest<TenantDto>;
}
