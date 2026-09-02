using GenbaseGoldReports.Application.Common.Interfaces;
using GenbaseGoldReports.Application.Features.Identity.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace GenbaseGoldReports.Application.Features.Identity.Commands.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
    {
        private readonly IMasterDbContext _masterDb;
        private readonly ITokenService _tokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LogoutCommandHandler(
            IMasterDbContext masterDb,
            ITokenService tokenService,
            IHttpContextAccessor httpContextAccessor)
        {
            _masterDb = masterDb;
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var rawToken = request.RefreshToken
                ?? _httpContextAccessor.HttpContext?.Request.Cookies["X-Refresh-Token"];

            if (!string.IsNullOrWhiteSpace(rawToken))
            {
                var tokenHash = _tokenService.HashRefreshToken(rawToken);

                var storedToken = await _masterDb.RefreshTokens
                    .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, cancellationToken);

                if (storedToken is { IsRevoked: false })
                {
                    storedToken.IsRevoked = true;
                    await _masterDb.SaveChangesAsync(cancellationToken);
                }
            }

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            _httpContextAccessor.HttpContext?.Response.Cookies.Delete("X-Refresh-Token", cookieOptions);
        }
    }
}
