using GenbaseGoldReports.Application.Common.Interfaces;
using GenbaseGoldReports.Domain.Entities.Masters;
using GenbaseGoldReports.Domain.Entities.Sales;
using Microsoft.EntityFrameworkCore;

namespace GenbaseGoldReports.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly ITenantContext _tenantContext;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ITenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<Docmst> Docmsts => Set<Docmst>();
    public DbSet<Msal> Msals => Set<Msal>();
    public DbSet<Msaldet> Msaldets => Set<Msaldet>();
    public DbSet<Accmst> Accmsts => Set<Accmst>();
    public DbSet<Actypes> Actypes => Set<Actypes>();
    public DbSet<Accdet> Accdets => Set<Accdet>();
    public DbSet<Classmst> Classmsts => Set<Classmst>();
    public DbSet<Currmst> Currmsts => Set<Currmst>();
    public DbSet<Configmst> Configmsts => Set<Configmst>();
    public DbSet<Metalmaster> Metalmasters => Set<Metalmaster>();
    public DbSet<Karatmst> Karatmsts => Set<Karatmst>();
    public DbSet<Mtltypemst> Mtltypemsts => Set<Mtltypemst>();
    public DbSet<Uommst> Uommsts => Set<Uommst>();
    public DbSet<Catmst> Catmsts => Set<Catmst>();
    public DbSet<Subcatmst> Subcatmsts => Set<Subcatmst>();
    public DbSet<Typemst> Typemsts => Set<Typemst>();
    public DbSet<Brandmst> Brandmsts => Set<Brandmst>();
    public DbSet<Lccmst> Lccmsts => Set<Lccmst>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        if (_tenantContext != null && _tenantContext.IsResolved && !string.IsNullOrEmpty(_tenantContext.ConnectionString))
        {
            optionsBuilder.UseSqlServer(_tenantContext.ConnectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly,
            type => type.Namespace != null && type.Namespace.EndsWith("Configurations.Application"));
    }
}