using GenbaseGoldReports.Application.Common.Exceptions;
using GenbaseGoldReports.Application.Common.Interfaces;
using GenbaseGoldReports.Application.Features.Tenants.Dtos;
using GenbaseGoldReports.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GenbaseGoldReports.Application.Features.Tenants.Commands.UpdateTenantSubscription
{
    public class UpdateTenantSubscriptionCommandHandler : IRequestHandler<UpdateTenantSubscriptionCommand, TenantDto>
    {
        private readonly IMasterDbContext _masterDb;

        public UpdateTenantSubscriptionCommandHandler(IMasterDbContext masterDb)
        {
            _masterDb = masterDb;
        }

        public async Task<TenantDto> Handle(UpdateTenantSubscriptionCommand request, CancellationToken cancellationToken)
        {
            var tenant = await _masterDb.Tenants
                .FirstOrDefaultAsync(t => t.Id == request.TenantId, cancellationToken);

            if (tenant == null)
            {
                throw new NotFoundException($"Tenant with ID '{request.TenantId}' was not found.");
            }

            DateTime? calculatedExpirationDateUtc = request.CustomExpirationDateUtc ?? request.SubscriptionPlan switch
            {
                TenantSubscriptionPlan.Trial => DateTime.UtcNow.AddDays(30),
                TenantSubscriptionPlan.Standard => DateTime.UtcNow.AddYears(1),
                TenantSubscriptionPlan.Enterprise => null, 
                _ => null
            };

            tenant.UpdateSubscription(request.SubscriptionPlan, calculatedExpirationDateUtc);

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
