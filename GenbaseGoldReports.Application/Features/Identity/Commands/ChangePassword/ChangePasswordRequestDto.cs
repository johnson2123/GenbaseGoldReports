namespace GenbaseGoldReports.Application.Features.Identity.Commands.ChangePassword
{
    public record ChangePasswordRequestDto(
        string CurrentPassword,
        string NewPassword,
        string ConfirmPassword
    );
}
