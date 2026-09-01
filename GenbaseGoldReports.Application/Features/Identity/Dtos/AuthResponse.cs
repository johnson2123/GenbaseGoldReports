namespace GenbaseGoldReports.Application.Features.Identity.Dtos
{
    public record AuthResponse(
        string AccessToken,
        string RefreshToken,
        DateTime AccessTokenExpiresAtUtc,
        DateTime RefreshTokenExpiresAtUtc,
        UserDto User);
}
