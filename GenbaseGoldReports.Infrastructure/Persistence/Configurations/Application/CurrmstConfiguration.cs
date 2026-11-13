using GenbaseGoldReports.Domain.Entities.Masters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenbaseGoldReports.Infrastructure.Persistence.Configurations.Application
{
    public class CurrmstConfiguration : IEntityTypeConfiguration<Currmst>
    {
        public void Configure(EntityTypeBuilder<Currmst> builder)
        {
            builder.ToTable("currmst");
            builder.HasKey(e => new { e.Currcode, e.Docno });

            builder.Property(e => e.Currcode).HasColumnName("currcode").HasMaxLength(10);
            builder.Property(e => e.Currname).HasColumnName("currname").HasMaxLength(50);
            builder.Property(e => e.Currrate).HasColumnName("currrate").HasPrecision(18, 5);
            builder.Property(e => e.Doctype).HasColumnName("doctype").HasMaxLength(10);
            builder.Property(e => e.Docno).HasColumnName("DOCNO");
            builder.Property(e => e.Slno).HasColumnName("slno").HasMaxLength(10);
            builder.Property(e => e.Docdate).HasColumnName("docdate");
            builder.Property(e => e.Curr).HasColumnName("curr").HasMaxLength(10);
            builder.Property(e => e.Mdoctype).HasColumnName("mdoctype").HasMaxLength(10);
            builder.Property(e => e.Sdoctype).HasColumnName("sdoctype").HasMaxLength(10);
            builder.Property(e => e.Subname).HasColumnName("subname").HasMaxLength(50);
            builder.Property(e => e.Brncode).HasColumnName("brncode").HasMaxLength(10);
            builder.Property(e => e.Loccode).HasColumnName("loccode").HasMaxLength(10);
            builder.Property(e => e.Usercode).HasColumnName("usercode").HasMaxLength(10);
        }
    }
}
