using GenbaseGoldReports.Domain.Entities.Masters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenbaseGoldReports.Infrastructure.Persistence.Configurations.Application
{
    public class MtltypemstConfiguration : IEntityTypeConfiguration<Mtltypemst>
    {
        public void Configure(EntityTypeBuilder<Mtltypemst> builder)
        {
            builder.ToTable("mtltypemst");
            builder.HasKey(e => e.Code);

            builder.Property(e => e.Code).HasColumnName("code").HasMaxLength(1).IsRequired();
            builder.Property(e => e.Mtltype).HasColumnName("mtltype").HasMaxLength(1);
            builder.Property(e => e.Description).HasColumnName("description").HasMaxLength(55);
            builder.Property(e => e.Loccode).HasColumnName("loccode").HasMaxLength(55);
            builder.Property(e => e.Usercode).HasColumnName("usercode").HasMaxLength(55);
        }
    }
}
