using GenbaseGoldReports.Application.Common.Interfaces;
using GenbaseGoldReports.Application.Features.Identity.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GenbaseGoldReports.Application.Features.Identity.Commands.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
    {
        private readonly IMasterDbContext _masterDb;
        private readonly ITokenService _tokenService;

        public LogoutCommandHandler(
            IMasterDbContext masterDb,
            ITokenService tokenService)
        {
            _masterDb = masterDb;
            _tokenService = tokenService;
        }

        public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return;

            var tokenHash = _tokenService.HashRefreshToken(request.RefreshToken);

            var storedToken = await _masterDb.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, cancellationToken);

            if (storedToken is { IsRevoked: false })
            {
                storedToken.IsRevoked = true;
                await _masterDb.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
