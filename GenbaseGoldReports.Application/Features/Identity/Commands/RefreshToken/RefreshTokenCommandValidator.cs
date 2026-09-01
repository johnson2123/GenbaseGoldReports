using FluentValidation;

namespace GenbaseGoldReports.Application.Features.Identity.Commands.RefreshToken
{
    public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(x => x.ExpiredAccessToken)
                .NotEmpty().WithMessage("Expired access token is required.");

            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required.");
        }
    }
}
