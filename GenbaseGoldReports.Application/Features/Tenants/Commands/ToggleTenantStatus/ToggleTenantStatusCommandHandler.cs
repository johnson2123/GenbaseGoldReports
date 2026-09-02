using GenbaseGoldReports.Application.Common.Exceptions;
using GenbaseGoldReports.Application.Common.Interfaces;
using GenbaseGoldReports.Application.Features.Tenants.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GenbaseGoldReports.Application.Features.Tenants.Commands.ToggleTenantStatus
{
    public class ToggleTenantStatusCommandHandler : IRequestHandler<ToggleTenantStatusCommand, TenantDto>
    {
        private readonly IMasterDbContext _masterDb;

        public ToggleTenantStatusCommandHandler(IMasterDbContext masterDb)
        {
            _masterDb = masterDb;
        }

        public async Task<TenantDto> Handle(ToggleTenantStatusCommand request, CancellationToken cancellationToken)
        {
            var tenant = await _masterDb.Tenants
                .FirstOrDefaultAsync(t => t.Id == request.TenantId, cancellationToken);

            if (tenant == null)
            {
                throw new NotFoundException($"Tenant with ID '{request.TenantId}' was not found.");
            }

            if (tenant.Identifier == "admin" && !request.IsActive)
            {
                throw new InvalidOperationException("The System Control Platform tenant cannot be deactivated.");
            }

            if (request.IsActive)
            {
                tenant.Activate();
            }
            else
            {
                tenant.Deactivate();
            }

            await _masterDb.SaveChangesAsync(cancellationToken);

            return new TenantDto(
                tenant.Id,
                tenant.Name,
                tenant.Identifier,
                tenant.IsActive,
                tenant.SubscriptionPlan,
                tenant.SubscriptionExpiresAtUtc,
                tenant.CreatedAtUtc,
                tenant.UpdatedAtUtc
            );
        }
    }
}
