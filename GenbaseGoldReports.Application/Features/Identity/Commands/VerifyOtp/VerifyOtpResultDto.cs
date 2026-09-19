namespace GenbaseGoldReports.Application.Features.Identity.Commands.VerifyOtp
{
    public record VerifyOtpResultDto(
        string ActionToken,
        string Target,
        DateTime ExpiresAtUtc
    );
}
