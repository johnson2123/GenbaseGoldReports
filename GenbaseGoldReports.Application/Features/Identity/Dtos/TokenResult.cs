namespace GenbaseGoldReports.Application.Features.Identity.Dtos
{
    public record TokenResult(string Token, DateTime ExpiresAtUtc);
}
