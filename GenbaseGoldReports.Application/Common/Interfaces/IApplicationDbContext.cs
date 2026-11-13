using GenbaseGoldReports.Domain.Entities.Masters;
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
        DbSet<Accmst> Accmsts { get; }
        DbSet<Actypes> Actypes { get; }
        DbSet<Accdet> Accdets { get; }
        DbSet<Classmst> Classmsts { get; }
        DbSet<Currmst> Currmsts { get; }
        DbSet<Configmst> Configmsts { get; }
        DbSet<Metalmaster> Metalmasters { get; }
        DbSet<Karatmst> Karatmsts { get; }
        DbSet<Mtltypemst> Mtltypemsts { get; }
        DbSet<Uommst> Uommsts { get; }
        DbSet<Catmst> Catmsts { get; }
        DbSet<Subcatmst> Subcatmsts { get; }
        DbSet<Typemst> Typemsts { get; }
        DbSet<Brandmst> Brandmsts { get; }
        DbSet<Lccmst> Lccmsts { get; }
        DatabaseFacade Database { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
