using GenbaseGoldReports.Application.Common.Interfaces;
using GenbaseGoldReports.Application.Features.Identity.Interfaces;
using GenbaseGoldReports.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GenbaseGoldReports.Application.Features.Identity.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOtpService _otpService;
        private readonly IMasterDbContext _masterDb;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ResetPasswordCommandHandler(
            UserManager<ApplicationUser> userManager,
            IOtpService otpService,
            IMasterDbContext masterDb,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _otpService = otpService;
            _masterDb = masterDb;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var sanitizedTarget = request.Target.Trim().ToLowerInvariant();

            var isTokenValid = await _otpService.ValidateActionTokenAsync(
                target: sanitizedTarget,
                token: request.ActionToken,
                purpose: "ForgotPassword",
                cancellationToken: cancellationToken
            );

            if (!isTokenValid)
            {
                throw new ArgumentException("The password reset token is invalid or has expired. Please request a new verification code.");
            }

            var user = await _userManager.FindByEmailAsync(sanitizedTarget);
            if (user == null || !user.IsActive)
            {
                throw new ArgumentException("User account not found or is disabled.");
            }

            var removeResult = await _userManager.RemovePasswordAsync(user);
            if (!removeResult.Succeeded)
            {
                var errors = string.Join(" ", removeResult.Errors.Select(e => e.Description));
                throw new ArgumentException($"Failed to reset password: {errors}");
            }

            var addResult = await _userManager.AddPasswordAsync(user, request.NewPassword);
            if (!addResult.Succeeded)
            {
                var errors = string.Join(" ", addResult.Errors.Select(e => e.Description));
                throw new ArgumentException($"Password requirements failed: {errors}");
            }

            await _userManager.ResetAccessFailedCountAsync(user);

            await _otpService.InvalidateActionTokenAsync(request.ActionToken, cancellationToken);

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
