using GenbaseGoldReports.Application.Common.Exceptions;
using GenbaseGoldReports.Application.Common.Interfaces;
using GenbaseGoldReports.Application.Features.Identity.Dtos;
using GenbaseGoldReports.Application.Features.Identity.Interfaces;
using GenbaseGoldReports.Domain.Entities.Identity;
using MediatR;
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

        public GetInitialSessionCommandHandler(
            IMasterDbContext masterDb,
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService,
            ITenantContext tenantContext)
        {
            _masterDb = masterDb;
            _userManager = userManager;
            _tokenService = tokenService;
            _tenantContext = tenantContext;
        }

        public async Task<AuthResponse> Handle(GetInitialSessionCommand request, CancellationToken cancellationToken)
        {
            if (!_tenantContext.IsResolved || string.IsNullOrEmpty(_tenantContext.Identifier))
            {
                throw new ForbiddenAccessException("A valid tenant domain header (X-Tenant-Id) or subdomain is required.");
            }

            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                throw new UnauthorizedAccessException("Session token is missing.");
            }

            var incomingHash = _tokenService.HashRefreshToken(request.RefreshToken);
            var storedToken = await _masterDb.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.TokenHash == incomingHash, cancellationToken);

            if (storedToken == null)
            {
                throw new UnauthorizedAccessException("Refresh token hash not found in Master DB.");
            }

            if (storedToken.ExpiresAtUtc <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Refresh token session has expired.");
            }

            // Concurrency Grace Window: Handle React 18 double-mount / concurrent reload requests
            if (storedToken.IsRevoked)
            {
                if (!string.IsNullOrEmpty(storedToken.ReplacedByTokenHash))
                {
                    var replacementToken = await _masterDb.RefreshTokens
                        .FirstOrDefaultAsync(rt => rt.TokenHash == storedToken.ReplacedByTokenHash, cancellationToken);

                    if (replacementToken != null && !replacementToken.IsRevoked && replacementToken.ExpiresAtUtc > DateTime.UtcNow)
                    {
                        storedToken = replacementToken;
                    }
                    else
                    {
                        throw new UnauthorizedAccessException("Refresh token was already revoked.");
                    }
                }
                else
                {
                    throw new UnauthorizedAccessException("Refresh token was already revoked.");
                }
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