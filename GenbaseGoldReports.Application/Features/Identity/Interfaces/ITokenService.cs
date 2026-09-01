using GenbaseGoldReports.Application.Features.Identity.Dtos;
using GenbaseGoldReports.Domain.Entities.Identity;
using System.Security.Claims;

namespace GenbaseGoldReports.Application.Features.Identity.Interfaces
{
    public interface ITokenService
    {
        TokenResult GenerateAccessToken(ApplicationUser user, IEnumerable<string> roles, IEnumerable<string> tenantIdentifiers);
        RefreshTokenResult GenerateRefreshToken();
        string HashRefreshToken(string refreshToken);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
