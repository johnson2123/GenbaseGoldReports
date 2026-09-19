using MediatR;

namespace GenbaseGoldReports.Application.Features.Identity.Commands.ChangePassword
{
    public record ChangePasswordCommand(
        Guid UserId,
        string CurrentPassword,
        string NewPassword,
        string ConfirmPassword
    ) : IRequest;
}
