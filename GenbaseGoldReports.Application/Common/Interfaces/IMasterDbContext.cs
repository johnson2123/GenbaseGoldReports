using GenbaseGoldReports.Domain.Entities;
using GenbaseGoldReports.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace GenbaseGoldReports.Application.Common.Interfaces
{
    public interface IMasterDbContext
    {
        DbSet<Tenant> Tenants { get; }
        DbSet<UserTenant> UserTenants { get; }
        DbSet<RefreshToken> RefreshTokens { get; }
        DbSet<OtpLog> OtpLogs { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
