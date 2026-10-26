using GenbaseGoldReports.Domain.Entities.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenbaseGoldReports.Infrastructure.Persistence.Configurations.Application
{
    public class DocmstConfiguration : IEntityTypeConfiguration<Docmst>
    {
        public void Configure(EntityTypeBuilder<Docmst> builder)
        {
            builder.ToTable("docmst");

            // Logical key for EF tracking
            builder.HasKey(x => new { x.Brncode, x.Divcode, x.Doctype });

            builder.Property(x => x.Brncode).HasColumnName("brncode").HasMaxLength(10);
            builder.Property(x => x.Divcode).HasColumnName("divcode").HasMaxLength(10);
            builder.Property(x => x.Doctype).HasColumnName("doctype").HasMaxLength(10);
            builder.Property(x => x.Lastdocno).HasColumnName("LASTDOCNO");
            builder.Property(x => x.Docprefix).HasColumnName("docprefix").HasMaxLength(20);
            builder.Property(x => x.Mdoctype).HasColumnName("mdoctype").HasMaxLength(10);
            builder.Property(x => x.Sdoctype).HasColumnName("sdoctype").HasMaxLength(10);
        }
    }
}
