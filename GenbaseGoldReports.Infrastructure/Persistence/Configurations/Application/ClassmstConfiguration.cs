using GenbaseGoldReports.Domain.Entities.Masters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenbaseGoldReports.Infrastructure.Persistence.Configurations.Application
{
    public class ClassmstConfiguration : IEntityTypeConfiguration<Classmst>
    {
        public void Configure(EntityTypeBuilder<Classmst> builder)
        {
            builder.ToTable("classmst");
            builder.HasKey(e => e.Code);

            builder.Property(e => e.Code).HasColumnName("code").HasMaxLength(10);
            builder.Property(e => e.Descript).HasColumnName("descript").HasMaxLength(100);
            builder.Property(e => e.Loccode).HasColumnName("loccode").HasMaxLength(10);
            builder.Property(e => e.Usercode).HasColumnName("usercode").HasMaxLength(10);
        }
    }
}
