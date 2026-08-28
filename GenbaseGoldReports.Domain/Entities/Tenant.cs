namespace GenbaseGoldReports.Domain.Entities
{
    public class Tenant
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Identifier { get; private set; } = string.Empty; // Subdomain 
        public string ConnectionString { get; private set; } = string.Empty;
        public bool IsActive { get; private set; }
        public virtual ICollection<Identity.UserTenant> UserTenants { get; private set; } = new List<Identity.UserTenant>();


        private Tenant() { } // Required for EF Core

        public Tenant(string name, string identifier, string connectionString)
        {
            Id = Guid.NewGuid();
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Identifier = identifier?.ToLowerInvariant() ?? throw new ArgumentNullException(nameof(identifier));
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            IsActive = true;
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
        public void UpdateConnectionString(string newConnectionString) =>
            ConnectionString = newConnectionString ?? throw new ArgumentNullException(nameof(newConnectionString));
    }
}
