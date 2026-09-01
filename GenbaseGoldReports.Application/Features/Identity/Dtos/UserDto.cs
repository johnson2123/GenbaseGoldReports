namespace GenbaseGoldReports.Application.Features.Identity.Dtos
{
    public record UserDto(
                Guid Id,
                string UserName,
                string Email,
                string FirstName,
                string LastName,
                string FullName,
                string? PhoneNumber,
                IEnumerable<string> Roles);
}
