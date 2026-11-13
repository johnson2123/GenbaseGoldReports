using GenbaseGoldReports.Domain.Entities.Masters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenbaseGoldReports.Infrastructure.Persistence.Configurations.Application
{
    public class LccmstConfiguration : IEntityTypeConfiguration<Lccmst>
    {
        public void Configure(EntityTypeBuilder<Lccmst> builder)
        {
            builder.ToTable("lccmst");
            builder.HasKey(e => e.Code);

            builder.Property(e => e.Code).HasColumnName("code").HasMaxLength(4).IsRequired();
            builder.Property(e => e.Description).HasColumnName("description").HasMaxLength(55);
            builder.Property(e => e.Doctype).HasColumnName("doctype").HasMaxLength(6);
            builder.Property(e => e.Docno).HasColumnName("DOCNO");
            builder.Property(e => e.Slno).HasColumnName("slno").HasMaxLength(6);
            builder.Property(e => e.Lcctype).HasColumnName("lcctype").HasMaxLength(1);
            builder.Property(e => e.Docdate).HasColumnName("docdate");
            builder.Property(e => e.Loccode).HasColumnName("loccode").HasMaxLength(55);
            builder.Property(e => e.Usercode).HasColumnName("usercode").HasMaxLength(55);
        }
    }
}
