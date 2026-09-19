using GenbaseGoldReports.Application.Common.Interfaces;
using GenbaseGoldReports.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GenbaseGoldReports.Application.Features.Identity.Commands.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMasterDbContext _masterDb;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChangePasswordCommandHandler(
            UserManager<ApplicationUser> userManager,
            IMasterDbContext masterDb,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _masterDb = masterDb;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null || !user.IsActive)
            {
                throw new UnauthorizedAccessException("User account not found or is disabled.");
            }

            var identityResult = await _userManager.ChangePasswordAsync(
                user,
                request.CurrentPassword,
                request.NewPassword);

            if (!identityResult.Succeeded)
            {
                var errors = string.Join(" ", identityResult.Errors.Select(e => e.Description));
                throw new ArgumentException(errors);
            }

            var activeTokens = await _masterDb.RefreshTokens
                .Where(rt => rt.UserId == user.Id && !rt.IsRevoked)
                .ToListAsync(cancellationToken);

            foreach (var token in activeTokens)
            {
                token.IsRevoked = true;
            }

            await _masterDb.SaveChangesAsync(cancellationToken);

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
