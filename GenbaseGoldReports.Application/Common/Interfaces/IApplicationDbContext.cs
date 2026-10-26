using GenbaseGoldReports.Domain.Entities.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace GenbaseGoldReports.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Docmst> Docmsts { get; }
        DbSet<Msal> Msals { get; }
        DbSet<Msaldet> Msaldets { get; }

        DatabaseFacade Database { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
