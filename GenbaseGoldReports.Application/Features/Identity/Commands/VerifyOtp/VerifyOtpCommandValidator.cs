using FluentValidation;

namespace GenbaseGoldReports.Application.Features.Identity.Commands.VerifyOtp
{
    public class VerifyOtpCommandValidator : AbstractValidator<VerifyOtpCommand>
    {
        public VerifyOtpCommandValidator()
        {
            RuleFor(x => x.Target)
                .NotEmpty()
                .WithMessage("Target (email or phone number) is required.");

            RuleFor(x => x.OtpCode)
                .NotEmpty()
                .WithMessage("Verification code is required.")
                .Matches(@"^\d{6}$")
                .WithMessage("Verification code must be exactly 6 digits.");

            RuleFor(x => x.Purpose)
                .NotEmpty()
                .WithMessage("Purpose is required.");
        }
    }
}
