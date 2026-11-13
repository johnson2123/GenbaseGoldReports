using GenbaseGoldReports.Domain.Entities.Masters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenbaseGoldReports.Infrastructure.Persistence.Configurations.Application
{
    public class KaratmstConfiguration : IEntityTypeConfiguration<Karatmst>
    {
        public void Configure(EntityTypeBuilder<Karatmst> builder)
        {
            builder.ToTable("karatmst");
            builder.HasKey(e => e.Code);

            builder.Property(e => e.Code).HasColumnName("code").HasMaxLength(2).IsRequired();
            builder.Property(e => e.Description).HasColumnName("description").HasMaxLength(55);
            builder.Property(e => e.Purity).HasColumnName("purity").HasPrecision(14, 5);
            builder.Property(e => e.Spurity).HasColumnName("spurity").HasPrecision(14, 5);
            builder.Property(e => e.Mfgpurity).HasColumnName("mfgpurity").HasPrecision(14, 5);
            builder.Property(e => e.Sipcode).HasColumnName("sipcode").HasMaxLength(55);
            builder.Property(e => e.Location).HasColumnName("location").HasMaxLength(55);
            builder.Property(e => e.Loccode).HasColumnName("loccode").HasMaxLength(55);
            builder.Property(e => e.Usercode).HasColumnName("usercode").HasMaxLength(55);
        }
    }
}
