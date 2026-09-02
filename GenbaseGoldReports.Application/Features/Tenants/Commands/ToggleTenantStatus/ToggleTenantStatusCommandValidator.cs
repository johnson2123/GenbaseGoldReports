using FluentValidation;

namespace GenbaseGoldReports.Application.Features.Tenants.Commands.ToggleTenantStatus
{
    public class ToggleTenantStatusCommandValidator : AbstractValidator<ToggleTenantStatusCommand>
    {
        public ToggleTenantStatusCommandValidator()
        {
            RuleFor(v => v.TenantId)
                .NotEmpty().WithMessage("Tenant ID is required.");
        }
    }
}
