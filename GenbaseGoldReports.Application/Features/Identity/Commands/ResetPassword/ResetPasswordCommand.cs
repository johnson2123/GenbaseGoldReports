using MediatR;

namespace GenbaseGoldReports.Application.Features.Identity.Commands.ResetPassword
{
    public record ResetPasswordCommand(
        string Target,
        string ActionToken,
        string NewPassword,
        string ConfirmPassword
    ) : IRequest;
}
