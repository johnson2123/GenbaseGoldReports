using GenbaseGoldReports.Application.Features.Identity.Dtos;
using MediatR;

namespace GenbaseGoldReports.Application.Features.Identity.Commands.Login
{
    public record LoginCommand(
    string Email,
    string Password) : IRequest<AuthResponse>;
}
