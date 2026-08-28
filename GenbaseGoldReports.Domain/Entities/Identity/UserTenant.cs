namespace GenbaseGoldReports.Domain.Entities.Identity
{
    public class UserTenant
    {
        public Guid UserId { get; private set; }
        public virtual ApplicationUser User { get; private set; } = null!;

        public Guid TenantId { get; private set; }
        public virtual Tenant Tenant { get; private set; } = null!;

        public string RoleInTenant { get; private set; } = string.Empty; // e.g., "TenantAdmin", "User"
        public DateTime GrantedAt { get; private set; } = DateTime.UtcNow;

        private UserTenant() { } 

        public UserTenant(Guid userId, Guid tenantId, string roleInTenant)
        {
            UserId = userId;
            TenantId = tenantId;
            RoleInTenant = roleInTenant ?? throw new ArgumentNullException(nameof(roleInTenant));
            GrantedAt = DateTime.UtcNow;
        }
    }
}
