using GenbaseGoldReports.Application.Common.Exceptions;
using GenbaseGoldReports.Application.Common.Interfaces;
using GenbaseGoldReports.Application.Features.Identity.Dtos;
using GenbaseGoldReports.Application.Features.Identity.Interfaces;
using GenbaseGoldReports.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RefreshTokenEntity = GenbaseGoldReports.Domain.Entities.Identity.RefreshToken;

namespace GenbaseGoldReports.Application.Features.Identity.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IMasterDbContext _masterDbContext;
    private readonly ITenantContext _tenantContext;

    public LoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        IMasterDbContext masterDbContext,
        ITenantContext tenantContext)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _masterDbContext = masterDbContext;
        _tenantContext = tenantContext;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        if (!_tenantContext.IsResolved || string.IsNullOrEmpty(_tenantContext.Identifier))
        {
            throw new ForbiddenAccessException("A valid tenant domain header (X-Tenant-Id) or subdomain is required.");
        }

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !user.IsActive)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        if (await _userManager.IsLockedOutAsync(user))
        {
            throw new ForbiddenAccessException("This account has been locked due to multiple failed login attempts.");
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            await _userManager.AccessFailedAsync(user);
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        var tenantIdentifiers = await _masterDbContext.UserTenants
            .AsNoTracking()
            .Where(ut => ut.UserId == user.Id && ut.Tenant.IsActive)
            .Select(ut => ut.Tenant.Identifier)
            .ToListAsync(cancellationToken);

        var isAuthorizedForActiveTenant = tenantIdentifiers.Contains(_tenantContext.Identifier, StringComparer.OrdinalIgnoreCase);
        if (!isAuthorizedForActiveTenant)
        {
            throw new ForbiddenAccessException($"User '{request.Email}' is not authorized to log into domain '{_tenantContext.Identifier}'.");
        }

        var roles = await _userManager.GetRolesAsync(user);

        var tokenResult = _tokenService.GenerateAccessToken(user, roles, tenantIdentifiers);
        var refreshTokenResult = _tokenService.GenerateRefreshToken();
        var hashedRefreshToken = _tokenService.HashRefreshToken(refreshTokenResult.Token);

        var refreshTokenEntity = new RefreshTokenEntity
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = hashedRefreshToken,
            ExpiresAtUtc = refreshTokenResult.ExpiresAtUtc,
            CreatedAtUtc = DateTime.UtcNow,
            IsRevoked = false
        };

        _masterDbContext.RefreshTokens.Add(refreshTokenEntity);
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
            tokenResult.Token,
            refreshTokenResult.Token,
            tokenResult.ExpiresAtUtc,
            refreshTokenResult.ExpiresAtUtc,
            userDto
        );
    }
}