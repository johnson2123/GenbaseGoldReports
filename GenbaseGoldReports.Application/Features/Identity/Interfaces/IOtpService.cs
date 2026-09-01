namespace GenbaseGoldReports.Application.Features.Identity.Interfaces
{
    public interface IOtpService
    {
        Task GenerateAndSendOtpAsync(string target, string communicationChannel, string purpose, CancellationToken cancellationToken = default);
        Task<string> VerifyOtpAsync(string target, string otpCode, string purpose, CancellationToken cancellationToken = default);
        Task<bool> ValidateActionTokenAsync(string target, string token, string purpose, CancellationToken cancellationToken = default);
        Task InvalidateActionTokenAsync(string token, CancellationToken cancellationToken = default);
    }
}
