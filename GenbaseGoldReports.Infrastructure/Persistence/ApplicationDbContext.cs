using GenbaseGoldReports.Application.Common.Interfaces;
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