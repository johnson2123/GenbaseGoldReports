using FluentValidation;

namespace GenbaseGoldReports.Application.Features.Identity.Commands.ResetPassword
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(x => x.Target)
                .NotEmpty()
                .WithMessage("Target (email or phone number) is required.");

            RuleFor(x => x.ActionToken)
                .NotEmpty()
                .WithMessage("Action token is required.");

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage("New password is required.")
                .MinimumLength(6)
                .WithMessage("Password must be at least 6 characters long.");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.NewPassword)
                .WithMessage("The new password and confirmation password do not match.");
        }
    }
}
