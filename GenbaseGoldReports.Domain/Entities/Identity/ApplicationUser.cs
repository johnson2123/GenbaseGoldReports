using Microsoft.AspNetCore.Identity;

namespace GenbaseGoldReports.Domain.Entities.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}".Trim();
        public bool IsActive { get; set; } = true;
        public bool MustChangePassword { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public virtual ICollection<UserTenant> UserTenants { get; set; } = new List<UserTenant>();
        public virtual ICollection<OtpLog> OtpLogs { get; set; } = new List<OtpLog>();
    }

}
