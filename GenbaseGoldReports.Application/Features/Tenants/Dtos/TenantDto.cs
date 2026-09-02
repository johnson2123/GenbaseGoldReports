using GenbaseGoldReports.Domain.Entities;

namespace GenbaseGoldReports.Application.Features.Tenants.Dtos
{
    public record TenantDto(
        Guid Id,
        string Name,
        string Identifier,
        bool IsActive,
        TenantSubscriptionPlan SubscriptionPlan,
        DateTime? SubscriptionExpiresAtUtc,
        DateTime CreatedAtUtc,
        DateTime? UpdatedAtUtc
    );
}
