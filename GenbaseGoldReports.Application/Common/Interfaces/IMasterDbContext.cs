using GenbaseGoldReports.Domain.Entities;
using GenbaseGoldReports.Domain.Entities.Identity;
using GenbaseGoldReports.Domain.Entities.Navigation;
using Microsoft.EntityFrameworkCore;

namespace GenbaseGoldReports.Application.Common.Interfaces
{
    public interface IMasterDbContext
    {
        DbSet<Tenant> Tenants { get; }
        DbSet<UserTenant> UserTenants { get; }
        DbSet<RefreshToken> RefreshTokens { get; }
        DbSet<OtpLog> OtpLogs { get; }
        DbSet<MenuItem> MenuItems { get; }
        DbSet<RoleMenuItem> RoleMenuItems { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
