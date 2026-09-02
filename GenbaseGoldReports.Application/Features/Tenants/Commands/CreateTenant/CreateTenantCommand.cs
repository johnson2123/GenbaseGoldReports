using GenbaseGoldReports.Application.Features.Tenants.Dtos;
using GenbaseGoldReports.Domain.Entities;
using MediatR;

namespace GenbaseGoldReports.Application.Features.Tenants.Commands.CreateTenant
{
    public record CreateTenantCommand(
        string Name,
        string Identifier, // Subdomain
        string AdminEmail,
        string AdminPassword,
        string AdminFirstName,
        string AdminLastName,
        string? CustomConnectionString = null,
        TenantSubscriptionPlan SubscriptionPlan = TenantSubscriptionPlan.Trial,
        DateTime? SubscriptionExpiresAtUtc = null
    ) : IRequest<TenantDto>;
}
