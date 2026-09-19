using MediatR;

namespace GenbaseGoldReports.Application.Features.Identity.Commands.ForgotPassword
{
    public record ForgotPasswordCommand(
    string Email
) : IRequest;
}
