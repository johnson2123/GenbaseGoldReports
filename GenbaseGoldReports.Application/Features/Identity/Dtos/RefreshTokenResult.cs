namespace GenbaseGoldReports.Application.Features.Identity.Dtos
{
    public record RefreshTokenResult(string Token, DateTime ExpiresAtUtc);
}
