using GenbaseGoldReports.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenbaseGoldReports.Infrastructure.Persistence.Configurations.Master
{
    public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
    {
        public void Configure(EntityTypeBuilder<Tenant> builder)
        {
            builder.ToTable("Tenants");
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Identifier)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(t => t.Identifier)
                .IsUnique();

            builder.Property(t => t.ConnectionString)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(t => t.IsActive)
                .IsRequired();
        }
    }
}
