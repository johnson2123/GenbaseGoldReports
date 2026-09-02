using GenbaseGoldReports.Application.Features.Tenants.Dtos;
using MediatR;

namespace GenbaseGoldReports.Application.Features.Tenants.Commands.ToggleTenantStatus
{
    public record ToggleTenantStatusCommand(Guid TenantId, bool IsActive) : IRequest<TenantDto>;
}
