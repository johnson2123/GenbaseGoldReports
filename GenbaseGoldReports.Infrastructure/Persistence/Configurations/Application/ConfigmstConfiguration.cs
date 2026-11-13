using GenbaseGoldReports.Domain.Entities.Masters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenbaseGoldReports.Infrastructure.Persistence.Configurations.Application
{
    public class ConfigmstConfiguration : IEntityTypeConfiguration<Configmst>
    {
        public void Configure(EntityTypeBuilder<Configmst> builder)
        {
            builder.ToTable("configmst");
            builder.HasNoKey();

            builder.Property(e => e.Basecurr).HasColumnName("basecurr").HasMaxLength(10);
            builder.Property(e => e.Lccurr).HasColumnName("lccurr").HasMaxLength(10);
            builder.Property(e => e.Lccurrrate).HasColumnName("lccurrrate").HasPrecision(18, 5);
            builder.Property(e => e.Mkgtaxrate).HasColumnName("mkgtaxrate").HasPrecision(18, 2);
            builder.Property(e => e.Loccode).HasColumnName("loccode").HasMaxLength(10);
            builder.Property(e => e.Usercode).HasColumnName("usercode").HasMaxLength(10);
        }
    }
}
