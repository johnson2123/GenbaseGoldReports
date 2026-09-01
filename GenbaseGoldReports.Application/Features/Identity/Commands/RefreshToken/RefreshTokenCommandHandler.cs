using GenbaseGoldReports.Application.Common.Interfaces;
using GenbaseGoldReports.Application.Features.Identity.Dtos;
using GenbaseGoldReports.Application.Features.Identity.Interfaces;
using GenbaseGoldReports.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using RefreshTokenEntity = GenbaseGoldReports.Domain.Entities.Identity.RefreshToken;

namespace GenbaseGoldReports.Application.Features.Identity.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IMasterDbContext _masterDbContext;

    public RefreshTokenCommandHandler(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        IMasterDbContext masterDbContext)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _masterDbContext = masterDbContext;
    }

    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(request.ExpiredAccessToken);
        if (principal == null)
        {
            throw new UnauthorizedAccessException("GetPrincipalFromExpiredToken failed. Signature algorithm mismatch or token corrupted.");
        }

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Malformed access token payload. NameIdentifier claim missing.");
        }

        var incomingHash = _tokenService.HashRefreshToken(request.RefreshToken);

        var storedToken = await _masterDbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.TokenHash == incomingHash, cancellationToken);

        if (storedToken == null)
        {
            throw new UnauthorizedAccessException("Refresh token hash not found in Master DB.");
        }

        if (storedToken.UserId != userId)
        {
            throw new UnauthorizedAccessException("Refresh token belongs to a different user.");
        }

        if (storedToken.IsRevoked)
        {
            throw new UnauthorizedAccessException("Refresh token has already been revoked.");
        }

        if (storedToken.ExpiresAtUtc <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Refresh token session has expired.");
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null || !user.IsActive)
        {
            throw new UnauthorizedAccessException("User account is disabled or missing.");
        }

        var roles = await _userManager.GetRolesAsync(user);

        var tenantIdentifiers = await _masterDbContext.UserTenants
            .AsNoTracking()
            .Where(ut => ut.UserId == user.Id && ut.Tenant.IsActive)
            .Select(ut => ut.Tenant.Identifier)
            .ToListAsync(cancellationToken);

        var newAccessToken = _tokenService.GenerateAccessToken(user, roles, tenantIdentifiers);
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        var newHashedToken = _tokenService.HashRefreshToken(newRefreshToken.Token);

        storedToken.IsRevoked = true;
        storedToken.ReplacedByTokenHash = newHashedToken;

        var newRefreshTokenEntity = new RefreshTokenEntity
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = newHashedToken,
            ExpiresAtUtc = newRefreshToken.ExpiresAtUtc,
            CreatedAtUtc = DateTime.UtcNow,
            IsRevoked = false
        };

        _masterDbContext.RefreshTokens.Add(newRefreshTokenEntity);
        await _masterDbContext.SaveChangesAsync(cancellationToken);

        var userDto = new UserDto(
            user.Id,
            user.UserName ?? user.Email!,
            user.Email!,
            user.FirstName,
            user.LastName,
            user.FullName,
            user.PhoneNumber,
            roles
        );

        return new AuthResponse(
            newAccessToken.Token,
            newRefreshToken.Token,
            newAccessToken.ExpiresAtUtc,
            newRefreshToken.ExpiresAtUtc,
            userDto
        );
    }
}