using GenbaseGoldReports.Application.Features.Identity.Dtos;
using MediatR;

namespace GenbaseGoldReports.Application.Features.Identity.Commands.RefreshToken
{
    
    public record RefreshTokenCommand(
        string ExpiredAccessToken,
        string RefreshToken) : IRequest<AuthResponse>;
}
