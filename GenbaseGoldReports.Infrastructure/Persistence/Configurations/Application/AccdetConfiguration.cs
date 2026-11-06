using GenbaseGoldReports.Domain.Entities.Masters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenbaseGoldReports.Infrastructure.Persistence.Configurations.Application
{
    public class AccdetConfiguration : IEntityTypeConfiguration<Accdet>
    {
        public void Configure(EntityTypeBuilder<Accdet> builder)
        {
            builder.ToTable("accdet");

            builder.HasKey(e => new { e.Accode, e.Curr });

            builder.Property(e => e.Accode).HasColumnName("accode").HasMaxLength(6);
            builder.Property(e => e.Curr).HasColumnName("curr").HasMaxLength(3);
            builder.Property(e => e.Doctype).HasColumnName("doctype").HasMaxLength(6);
            builder.Property(e => e.Docno).HasColumnName("docno").HasMaxLength(8);
            builder.Property(e => e.Docdoctype).HasColumnName("docdoctype").HasMaxLength(6);
            builder.Property(e => e.Mdoctype).HasColumnName("mdoctype").HasMaxLength(5);
            builder.Property(e => e.Sdoctype).HasColumnName("sdoctype").HasMaxLength(5);
            builder.Property(e => e.Currname).HasColumnName("currname").HasMaxLength(55);
            builder.Property(e => e.Docdate).HasColumnName("docdate");
            builder.Property(e => e.Levelcode).HasColumnName("levelcode").HasMaxLength(2);
            builder.Property(e => e.Uniqueid).HasColumnName("uniqueid").HasPrecision(14, 2);
            builder.Property(e => e.Loccode).HasColumnName("loccode").HasMaxLength(55);
            builder.Property(e => e.Usercode).HasColumnName("usercode").HasMaxLength(55);
        }
    }
}
