using GenbaseGoldReports.Domain.Entities;
using GenbaseGoldReports.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GenbaseGoldReports.Infrastructure.Persistence;

public class MasterDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public MasterDbContext(DbContextOptions<MasterDbContext> options) : base(options)
    {
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<UserTenant> UserTenants => Set<UserTenant>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<OtpLog> OtpLogs => Set<OtpLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(
            typeof(MasterDbContext).Assembly,
            type => type.Namespace != null && type.Namespace.EndsWith("Configurations.Master"));
    }
}