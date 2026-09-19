using GenbaseGoldReports.Application.Features.Identity.Interfaces;
using MediatR;

namespace GenbaseGoldReports.Application.Features.Identity.Commands.VerifyOtp
{
    public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, VerifyOtpResultDto>
    {
        private readonly IOtpService _otpService;

        public VerifyOtpCommandHandler(IOtpService otpService)
        {
            _otpService = otpService;
        }

        public async Task<VerifyOtpResultDto> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            var sanitizedTarget = request.Target.Trim().ToLowerInvariant();
            var sanitizedOtp = request.OtpCode.Trim();

            var actionToken = await _otpService.VerifyOtpAsync(
                target: sanitizedTarget,
                otpCode: sanitizedOtp,
                purpose: request.Purpose,
                cancellationToken: cancellationToken
            );

            return new VerifyOtpResultDto(
                ActionToken: actionToken,
                Target: sanitizedTarget,
                ExpiresAtUtc: DateTime.UtcNow.AddMinutes(10)
            );
        }
    }
}
