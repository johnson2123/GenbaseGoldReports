namespace GenbaseGoldReports.Domain.Entities.Identity
{
    public class OtpLog
    {
        public int Id { get; set; }
        public Guid Guid { get; set; } = Guid.NewGuid();

        public Guid ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; } = null!;

        public string Target { get; set; } = string.Empty; // Email or Mobile Number
        public string OtpCode { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty; // e.g. "ForgotPassword", "TwoFactorAuth"

        public DateTime ExpiryTime { get; set; }
        public bool IsUsed { get; set; }
        public int AttemptCount { get; set; }

        public string? ActionToken { get; set; }
        public DateTime? ActionTokenExpiry { get; set; }
        public bool IsConsumed { get; set; }

        public string? IPAddress { get; set; }
        public string? DeviceInfo { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? VerifiedDate { get; set; }
        public DateTime? ConsumedDate { get; set; }
    }
}
