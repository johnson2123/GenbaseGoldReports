using FluentValidation;

namespace GenbaseGoldReports.Application.Features.Tenants.Commands.CreateTenant
{
    public class CreateTenantCommandValidator : AbstractValidator<CreateTenantCommand>
    {
        public CreateTenantCommandValidator()
        {
            RuleFor(v => v.Name)
                .NotEmpty().WithMessage("Tenant name is required.")
                .MaximumLength(100).WithMessage("Tenant name must not exceed 100 characters.");

            RuleFor(v => v.Identifier)
                .NotEmpty().WithMessage("Tenant identifier (subdomain) is required.")
                .MaximumLength(50).WithMessage("Tenant identifier must not exceed 50 characters.")
                .Matches("^[a-zA-Z0-9-]+$").WithMessage("Tenant identifier can only contain alphanumeric characters and hyphens.");

            RuleFor(v => v.AdminEmail)
                .NotEmpty().WithMessage("Admin email is required.")
                .EmailAddress().WithMessage("A valid email address is required.");

            RuleFor(v => v.AdminPassword)
                .NotEmpty().WithMessage("Admin password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");

            RuleFor(v => v.AdminFirstName)
                .NotEmpty().WithMessage("Admin first name is required.");

            RuleFor(v => v.AdminLastName)
                .NotEmpty().WithMessage("Admin last name is required.");
        }
    }
}
