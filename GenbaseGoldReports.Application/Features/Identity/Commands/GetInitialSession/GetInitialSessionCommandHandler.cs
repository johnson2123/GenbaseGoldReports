using GenbaseGoldReports.Application.Common.Exceptions;
using GenbaseGoldReports.Application.Common.Interfaces;
using GenbaseGoldReports.Application.Features.Identity.Dtos;
using GenbaseGoldReports.Application.Features.Identity.Interfaces;
using GenbaseGoldReports.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RefreshTokenEntity = GenbaseGoldReports.Domain.Entities.Identity.RefreshToken;

namespace GenbaseGoldReports.Application.Features.Identity.Commands.GetInitialSession
{
    public class GetInitialSessionCommandHandler : IRequestHandler<GetInitialSessionCommand, AuthResponse>
    {
        private readonly IMasterDbContext _masterDb;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly ITenantContext _tenantContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetInitialSessionCommandHandler(
            IMasterDbContext masterDb,
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService,
            ITenantContext tenantContext,
            IHttpContextAccessor httpContextAccessor)
        {
            _masterDb = masterDb;
            _userManager = userManager;
            _tokenService = tokenService;
            _tenantContext = tenantContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AuthResponse> Handle(GetInitialSessionCommand request, CancellationToken cancellationToken)
        {
            if (!_tenantContext.IsResolved || string.IsNullOrEmpty(_tenantContext.Identifier))
            {
                throw new ForbiddenAccessException("A valid tenant domain header (X-Tenant-Id) or subdomain is required.");
            }

            var rawRefreshToken = _httpContextAccessor.HttpContext?.Request.Cookies["X-Refresh-Token"];
            if (string.IsNullOrWhiteSpace(rawRefreshToken))
            {
                throw new UnauthorizedAccessException("Session cookie is missing.");
            }

            var incomingHash = _tokenService.HashRefreshToken(rawRefreshToken);
            var storedToken = await _masterDb.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.TokenHash == incomingHash, cancellationToken);

            if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiresAtUtc <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Invalid, expired, or revoked refresh token session.");
            }

            var user = await _userManager.FindByIdAsync(storedToken.UserId.ToString());
            if (user == null || !user.IsActive)
            {
                throw new UnauthorizedAccessException("User account is disabled or missing.");
            }

            var tenantIdentifiers = await _masterDb.UserTenants
                .AsNoTracking()
                .Where(ut => ut.UserId == user.Id && ut.Tenant.IsActive)
                .Select(ut => ut.Tenant.Identifier)
                .ToListAsync(cancellationToken);

            var isAuthorizedForActiveTenant = tenantIdentifiers.Contains(_tenantContext.Identifier, StringComparer.OrdinalIgnoreCase);
            if (!isAuthorizedForActiveTenant)
            {
                throw new ForbiddenAccessException($"User '{user.Email}' is not authorized to log into domain '{_tenantContext.Identifier}'.");
            }

            var roles = await _userManager.GetRolesAsync(user);

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

            _masterDb.RefreshTokens.Add(newRefreshTokenEntity);
            await _masterDb.SaveChangesAsync(cancellationToken);

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
}
