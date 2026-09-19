using GenbaseGoldReports.Application.Features.Identity.Interfaces;
using GenbaseGoldReports.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace GenbaseGoldReports.Application.Features.Identity.Commands.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOtpService _otpService;

        public ForgotPasswordCommandHandler(
            UserManager<ApplicationUser> userManager,
            IOtpService otpService)
        {
            _userManager = userManager;
            _otpService = otpService;
        }

        public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var sanitizedEmail = request.Email.Trim().ToLowerInvariant();

            var user = await _userManager.FindByEmailAsync(sanitizedEmail);

            if (user == null || !user.IsActive)
            {
                return;
            }

            await _otpService.GenerateAndSendOtpAsync(
                target: sanitizedEmail,
                communicationChannel: "All",
                purpose: "ForgotPassword",
                cancellationToken: cancellationToken
            );
        }
    }
}
