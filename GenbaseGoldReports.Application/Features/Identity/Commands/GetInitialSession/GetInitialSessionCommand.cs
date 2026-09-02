using GenbaseGoldReports.Application.Features.Identity.Dtos;
using MediatR;

namespace GenbaseGoldReports.Application.Features.Identity.Commands.GetInitialSession
{
    public record GetInitialSessionCommand : IRequest<AuthResponse>;
}
