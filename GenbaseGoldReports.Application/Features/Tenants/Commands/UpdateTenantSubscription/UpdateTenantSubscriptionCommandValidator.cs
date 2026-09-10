using FluentValidation;

namespace GenbaseGoldReports.Application.Features.Tenants.Commands.UpdateTenantSubscription
{
    public class UpdateTenantSubscriptionCommandValidator : AbstractValidator<UpdateTenantSubscriptionCommand>
    {
        public UpdateTenantSubscriptionCommandValidator()
        {
            RuleFor(v => v.TenantId)
                .NotEmpty().WithMessage("Tenant ID is required.");

            RuleFor(v => v.SubscriptionPlan)
                .IsInEnum().WithMessage("A valid subscription plan (Trial, Standard, or Enterprise) must be specified.");

            RuleFor(v => v.CustomExpirationDateUtc)
                .Must(date => !date.HasValue || date.Value > DateTime.UtcNow)
                .WithMessage("Custom expiration date must be in the future.");
        }
    }
}
