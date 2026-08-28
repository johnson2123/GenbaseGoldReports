namespace GenbaseGoldReports.Domain.Entities.Identity
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string TokenHash { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public virtual ApplicationUser User { get; set; } = null!;
        public DateTime ExpiresAtUtc { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public bool IsRevoked { get; set; }
        public string? ReplacedByTokenHash { get; set; }

        public bool IsActive => !IsRevoked && DateTime.UtcNow <= ExpiresAtUtc;
    }
}
