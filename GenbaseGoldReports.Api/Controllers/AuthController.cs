using GenbaseGoldReports.Application.Common.Models;
using GenbaseGoldReports.Application.Features.Identity.Commands.Login;
using GenbaseGoldReports.Application.Features.Identity.Commands.RefreshToken;
using GenbaseGoldReports.Application.Features.Identity.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenbaseGoldReports.Api.Controllers;

public class AuthController : BaseApiController
{
    private readonly ISender _mediator;

    public AuthController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        AuthResponse result = await _mediator.Send(command, cancellationToken);

        SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiresAtUtc);

        var response = ApiResponse<AuthResponse>.Success(
            data: result,
            message: "Authentication successful."
        );

        return Ok(response);
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request, CancellationToken cancellationToken)
    {
        if (!Request.Cookies.TryGetValue("X-Refresh-Token", out var currentRefreshToken) || string.IsNullOrWhiteSpace(currentRefreshToken))
        {
            return BadRequest(ApiResponse<object>.Failure(
                error: "Missing refresh token cookie.",
                message: "Refresh token is missing from request credentials."
            ));
        }

        var command = new RefreshTokenCommand(request.ExpiredAccessToken, currentRefreshToken);
        AuthResponse result = await _mediator.Send(command, cancellationToken);

        SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiresAtUtc);

        var response = ApiResponse<AuthResponse>.Success(
            data: result,
            message: "Token refreshed successfully."
        );

        return Ok(response);
    }

    private void SetRefreshTokenCookie(string token, DateTime expiresAtUtc)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = expiresAtUtc
        };

        Response.Cookies.Append("X-Refresh-Token", token, cookieOptions);
    }
}