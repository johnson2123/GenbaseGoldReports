using MediatR;

namespace GenbaseGoldReports.Application.Features.Identity.Commands.Logout
{
    public record LogoutCommand(string? RefreshToken) : IRequest;
}
