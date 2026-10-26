using GenbaseGoldReports.Domain.Entities.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenbaseGoldReports.Infrastructure.Persistence.Configurations.Application
{
    public class MsaldetConfiguration : IEntityTypeConfiguration<Msaldet>
    {
        public void Configure(EntityTypeBuilder<Msaldet> builder)
        {
            builder.ToTable("msaldet");

            builder.HasKey(x => new { x.Brncode, x.Divcode, x.Doctype, x.Docno, x.Slno });

            builder.Property(x => x.Brncode).HasColumnName("brncode").HasMaxLength(10);
            builder.Property(x => x.Divcode).HasColumnName("divcode").HasMaxLength(10);
            builder.Property(x => x.Doctype).HasColumnName("doctype").HasMaxLength(10);
            builder.Property(x => x.Docno).HasColumnName("DOCNO");
            builder.Property(x => x.Slno).HasColumnName("slno");
            builder.Property(x => x.Slno2).HasColumnName("slno2");
            builder.Property(x => x.Nfactor).HasColumnName("nfactor").HasPrecision(18, 2);

            builder.Property(x => x.Mdoctype).HasColumnName("mdoctype").HasMaxLength(10);
            builder.Property(x => x.Sdoctype).HasColumnName("sdoctype").HasMaxLength(10);
            builder.Property(x => x.Docdate).HasColumnName("docdate");
            builder.Property(x => x.Stimestamp).HasColumnName("stimestamp").HasMaxLength(20);
            builder.Property(x => x.Loccode).HasColumnName("loccode").HasMaxLength(20);
            builder.Property(x => x.Usercode).HasColumnName("usercode").HasMaxLength(50);
            builder.Property(x => x.Partycode).HasColumnName("partycode").HasMaxLength(50);
            builder.Property(x => x.Impexptype).HasColumnName("impexptype").HasMaxLength(20);
            builder.Property(x => x.Fixunfix).HasColumnName("fixunfix").HasMaxLength(10);
            builder.Property(x => x.Sman).HasColumnName("sman").HasMaxLength(50);

            builder.Property(x => x.Stkcode).HasColumnName("stkcode").HasMaxLength(50);
            builder.Property(x => x.Stksubcode).HasColumnName("stksubcode").HasMaxLength(50);
            builder.Property(x => x.Pieces).HasColumnName("pieces");
            builder.Property(x => x.Pcs).HasColumnName("pcs");
            builder.Property(x => x.Grqty).HasColumnName("grqty").HasPrecision(18, 3);
            builder.Property(x => x.Apurity).HasColumnName("apurity").HasPrecision(18, 5);
            builder.Property(x => x.Spurity).HasColumnName("spurity").HasPrecision(18, 5);
            builder.Property(x => x.Puqty).HasColumnName("puqty").HasPrecision(18, 3);

            builder.Property(x => x.Mkgrate).HasColumnName("mkgrate").HasPrecision(18, 2);
            builder.Property(x => x.Mkgamt).HasColumnName("mkgamt").HasPrecision(18, 2);
            builder.Property(x => x.Mtlrate).HasColumnName("mtlrate").HasPrecision(18, 4);
            builder.Property(x => x.Mtlamt).HasColumnName("mtlamt").HasPrecision(18, 2);
            builder.Property(x => x.Mkgtaxamt).HasColumnName("mkgtaxamt").HasPrecision(18, 2);
            builder.Property(x => x.Mtltaxamt).HasColumnName("mtltaxamt").HasPrecision(18, 2);
            builder.Property(x => x.Taxamt).HasColumnName("taxamt").HasPrecision(18, 2);
            builder.Property(x => x.Totalamt).HasColumnName("totalamt").HasPrecision(18, 2);

            builder.Property(x => x.Currcode).HasColumnName("currcode").HasMaxLength(10);
            builder.Property(x => x.Currrate).HasColumnName("currrate").HasPrecision(18, 5);
        }
    }
}
