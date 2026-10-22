using GenbaseGoldReports.Application.Common.Models;
using GenbaseGoldReports.Application.Features.Identity.Commands.ChangePassword;
using GenbaseGoldReports.Application.Features.Identity.Commands.ForgotPassword;
using GenbaseGoldReports.Application.Features.Identity.Commands.GetInitialSession;
using GenbaseGoldReports.Application.Features.Identity.Commands.Login;
using GenbaseGoldReports.Application.Features.Identity.Commands.Logout;
using GenbaseGoldReports.Application.Features.Identity.Commands.RefreshToken;
using GenbaseGoldReports.Application.Features.Identity.Commands.ResetPassword;
using GenbaseGoldReports.Application.Features.Identity.Commands.VerifyOtp;
using GenbaseGoldReports.Application.Features.Identity.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenbaseGoldReports.Api.Controllers
{
    public class AuthController : BaseApiController
    {
        private readonly ISender _mediator;
        private readonly IWebHostEnvironment _env;

        public AuthController(ISender mediator, IWebHostEnvironment env)
        {
            _mediator = mediator;
            _env = env;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiresAtUtc);

            var response = ApiResponse<AuthResponse>.Success(
                data: result,
                message: "Authentication successful."
            );

            return Ok(response);
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request, CancellationToken cancellationToken)
        {
            if (!Request.Cookies.TryGetValue("X-Refresh-Token", out var currentRefreshToken) || string.IsNullOrWhiteSpace(currentRefreshToken))
            {
                return BadRequest(ApiResponse<object>.Failure(
                    error: "Missing refresh token cookie.",
                    message: "Refresh token is missing from request credentials."
                ));
            }

            var command = new RefreshTokenCommand(request.ExpiredAccessToken, currentRefreshToken.Trim());
            var result = await _mediator.Send(command, cancellationToken);

            SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiresAtUtc);

            var response = ApiResponse<AuthResponse>.Success(
                data: result,
                message: "Token refreshed successfully."
            );

            return Ok(response);
        }

        [HttpGet("me")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetInitialSession(CancellationToken cancellationToken)
        {
            if (!Request.Cookies.TryGetValue("X-Refresh-Token", out var rawRefreshToken) || string.IsNullOrWhiteSpace(rawRefreshToken))
            {
                return Unauthorized(ApiResponse<object>.Failure(
                    error: "Session cookie is missing.",
                    message: "Refresh token cookie was not provided."
                ));
            }

            var command = new GetInitialSessionCommand(rawRefreshToken.Trim());
            var result = await _mediator.Send(command, cancellationToken);

            SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiresAtUtc);

            var response = ApiResponse<AuthResponse>.Success(
                data: result,
                message: "Session restored successfully."
            );

            return Ok(response);
        }

        [HttpPost("logout")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout([FromBody] LogoutCommand? command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command ?? new LogoutCommand(null), cancellationToken);

            var response = ApiResponse<string>.Success(
                data: string.Empty,
                message: "Logged out successfully."
            );

            return Ok(response);
        }

        [Authorize]
        [HttpPost("change-password")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword(
            [FromBody] ChangePasswordRequestDto dto,
            CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(CurrentUserId, out var userId))
            {
                return Unauthorized(ApiResponse<object>.Failure("Invalid or missing user context."));
            }

            var command = new ChangePasswordCommand(
                UserId: userId,
                CurrentPassword: dto.CurrentPassword,
                NewPassword: dto.NewPassword,
                ConfirmPassword: dto.ConfirmPassword
            );

            await _mediator.Send(command, cancellationToken);

            return Ok(ApiResponse<string>.Success(
                data: string.Empty,
                message: "Password changed successfully. Please log in with your new password."
            ));
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ForgotPassword(
            [FromBody] ForgotPasswordCommand command,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(command, cancellationToken);

            return Ok(ApiResponse<string>.Success(
                data: string.Empty,
                message: "If an account matching the provided email exists, a verification code has been dispatched."
            ));
        }

        [HttpPost("verify-otp")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<VerifyOtpResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> VerifyOtp(
            [FromBody] VerifyOtpCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(ApiResponse<VerifyOtpResultDto>.Success(
                data: result,
                message: "Verification successful. You may now reset your password."
            ));
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword(
            [FromBody] ResetPasswordCommand command,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(command, cancellationToken);

            return Ok(ApiResponse<string>.Success(
                data: string.Empty,
                message: "Your password has been reset successfully. Please log in with your new credentials."
            ));
        }

        private void SetRefreshTokenCookie(string token, DateTime expiresAtUtc)
        {
            var isDev = _env.IsDevelopment();
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, 
                SameSite = isDev ? SameSiteMode.None : SameSiteMode.Lax,
                Expires = expiresAtUtc,
                Path = "/" 
            };

            Response.Cookies.Append("X-Refresh-Token", token, cookieOptions);
        }
    }
}