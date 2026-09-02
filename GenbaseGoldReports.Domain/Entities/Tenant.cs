namespace GenbaseGoldReports.Domain.Entities
{
    public class Tenant
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Identifier { get; private set; } = string.Empty; 
        public string ConnectionString { get; private set; } = string.Empty;
        public bool IsActive { get; private set; }

        public TenantSubscriptionPlan SubscriptionPlan { get; private set; }
        public DateTime? SubscriptionExpiresAtUtc { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime? UpdatedAtUtc { get; private set; }

        public virtual ICollection<Identity.UserTenant> UserTenants { get; private set; } = new List<Identity.UserTenant>();

        private Tenant() { }

        public Tenant(
            string name,
            string identifier,
            string connectionString,
            TenantSubscriptionPlan subscriptionPlan = TenantSubscriptionPlan.Trial,
            DateTime? subscriptionExpiresAtUtc = null)
        {
            Id = Guid.NewGuid();
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Identifier = identifier?.ToLowerInvariant() ?? throw new ArgumentNullException(nameof(identifier));
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            IsActive = true;
            SubscriptionPlan = subscriptionPlan;

            SubscriptionExpiresAtUtc = subscriptionExpiresAtUtc
                ?? (subscriptionPlan == TenantSubscriptionPlan.Trial ? DateTime.UtcNow.AddDays(30) : null);

            CreatedAtUtc = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void UpdateSubscription(TenantSubscriptionPlan newPlan, DateTime? newExpirationDateUtc)
        {
            SubscriptionPlan = newPlan;
            SubscriptionExpiresAtUtc = newExpirationDateUtc;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void UpdateConnectionString(string newConnectionString)
        {
            ConnectionString = newConnectionString ?? throw new ArgumentNullException(nameof(newConnectionString));
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public bool IsSubscriptionValid => IsActive &&
            (!SubscriptionExpiresAtUtc.HasValue || SubscriptionExpiresAtUtc.Value >= DateTime.UtcNow);
    }
}
