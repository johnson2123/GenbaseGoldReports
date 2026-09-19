using MediatR;

namespace GenbaseGoldReports.Application.Features.Identity.Commands.VerifyOtp
{
    public record VerifyOtpCommand(
        string Target,
        string OtpCode,
        string Purpose
    ) : IRequest<VerifyOtpResultDto>;
}
