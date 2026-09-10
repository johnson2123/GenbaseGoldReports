using GenbaseGoldReports.Domain.Entities;

namespace GenbaseGoldReports.Application.Features.Tenants.Dtos
{
    public record UpdateTenantSubscriptionRequestBody(
        TenantSubscriptionPlan SubscriptionPlan
    );
}
