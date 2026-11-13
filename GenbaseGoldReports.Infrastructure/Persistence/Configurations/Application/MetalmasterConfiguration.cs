using GenbaseGoldReports.Domain.Entities.Masters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenbaseGoldReports.Infrastructure.Persistence.Configurations.Application
{
    public class MetalmasterConfiguration : IEntityTypeConfiguration<Metalmaster>
    {
        public void Configure(EntityTypeBuilder<Metalmaster> builder)
        {
            builder.ToTable("metalmaster");

            builder.HasKey(e => e.Stkcode);

            // Core Strings & Keys
            builder.Property(e => e.Stkcode).HasColumnName("stkcode").HasMaxLength(55).IsRequired();
            builder.Property(e => e.Stksubcode).HasColumnName("stksubcode").HasMaxLength(55);
            builder.Property(e => e.Doctype).HasColumnName("doctype").HasMaxLength(6);
            builder.Property(e => e.Docno).HasColumnName("DOCNO");
            builder.Property(e => e.Slno).HasColumnName("slno").HasMaxLength(6);
            builder.Property(e => e.Docdate).HasColumnName("docdate");
            builder.Property(e => e.Code).HasColumnName("code").HasMaxLength(4);
            builder.Property(e => e.Description).HasColumnName("description").HasMaxLength(55);
            builder.Property(e => e.Stkname).HasColumnName("stkname").HasMaxLength(55);
            builder.Property(e => e.Stkdetails).HasColumnName("stkdetails").HasMaxLength(55);

            // Classifications & Lookups
            builder.Property(e => e.Mtltype).HasColumnName("mtltype").HasMaxLength(1);
            builder.Property(e => e.Karat).HasColumnName("karat").HasMaxLength(2);
            builder.Property(e => e.Uom).HasColumnName("uom").HasMaxLength(3);
            builder.Property(e => e.Cat).HasColumnName("cat").HasMaxLength(8);
            builder.Property(e => e.Subcat).HasColumnName("subcat").HasMaxLength(4);
            builder.Property(e => e.Type).HasColumnName("type").HasMaxLength(4);
            builder.Property(e => e.Brand).HasColumnName("brand").HasMaxLength(4);
            builder.Property(e => e.Lcccode).HasColumnName("lcccode").HasMaxLength(4);
            builder.Property(e => e.Mkglcccode).HasColumnName("mkglcccode").HasMaxLength(4);
            builder.Property(e => e.Tray).HasColumnName("tray").HasMaxLength(12);
            builder.Property(e => e.Ctry).HasColumnName("ctry").HasMaxLength(4);
            builder.Property(e => e.Piclocation).HasColumnName("piclocation").HasMaxLength(180);

            // Purities (5-decimal precision)
            builder.Property(e => e.Spurity).HasColumnName("spurity").HasPrecision(14, 5);
            builder.Property(e => e.Salpurity).HasColumnName("salpurity").HasPrecision(14, 5);
            builder.Property(e => e.Apurity).HasColumnName("apurity").HasPrecision(14, 2);

            // Pricing, Costing & Currency
            builder.Property(e => e.Cost).HasColumnName("cost").HasPrecision(14, 2);
            builder.Property(e => e.Costfc).HasColumnName("costfc").HasPrecision(14, 2);
            builder.Property(e => e.Costunit).HasColumnName("costunit").HasMaxLength(4);
            builder.Property(e => e.Curr).HasColumnName("curr").HasMaxLength(4);
            builder.Property(e => e.Currrate).HasColumnName("currrate").HasPrecision(14, 5);
            builder.Property(e => e.Mkgrate).HasColumnName("mkgrate").HasPrecision(14, 2);
            builder.Property(e => e.Priceunit).HasColumnName("priceunit").HasMaxLength(4);

            // Pricing Tiers (Legacy)
            builder.Property(e => e.Price1).HasColumnName("price1").HasPrecision(14, 2);
            builder.Property(e => e.Price1per).HasColumnName("price1per").HasPrecision(14, 2);
            builder.Property(e => e.Price1fc).HasColumnName("price1fc").HasPrecision(14, 2);
            builder.Property(e => e.Price1unit).HasColumnName("price1unit").HasMaxLength(4);

            builder.Property(e => e.Price2).HasColumnName("price2").HasPrecision(14, 2);
            builder.Property(e => e.Price2per).HasColumnName("price2per").HasPrecision(14, 2);
            builder.Property(e => e.Price2fc).HasColumnName("price2fc").HasPrecision(14, 2);
            builder.Property(e => e.Price2unit).HasColumnName("price2unit").HasMaxLength(4);

            builder.Property(e => e.Price3).HasColumnName("price3").HasPrecision(14, 2);
            builder.Property(e => e.Price3per).HasColumnName("price3per").HasPrecision(14, 2);
            builder.Property(e => e.Price3fc).HasColumnName("price3fc").HasPrecision(14, 2);
            builder.Property(e => e.Price3unit).HasColumnName("price3unit").HasMaxLength(4);

            builder.Property(e => e.Price4).HasColumnName("price4").HasPrecision(14, 2);
            builder.Property(e => e.Price4per).HasColumnName("price4per").HasPrecision(14, 2);
            builder.Property(e => e.Price4fc).HasColumnName("price4fc").HasPrecision(14, 2);

            // Taxes
            builder.Property(e => e.Taxrate).HasColumnName("taxrate").HasPrecision(14, 2);
            builder.Property(e => e.Mtltaxrate).HasColumnName("mtltaxrate").HasPrecision(14, 2);
            builder.Property(e => e.Mkgtaxrate).HasColumnName("mkgtaxrate").HasPrecision(14, 2);

            // Quantities & Secondary UOMs
            builder.Property(e => e.Pcweight).HasColumnName("pcweight").HasPrecision(14, 2);
            builder.Property(e => e.Suom).HasColumnName("suom").HasMaxLength(3);
            builder.Property(e => e.Mainuom).HasColumnName("mainuom").HasMaxLength(4);
            builder.Property(e => e.Subuom).HasColumnName("subuom").HasMaxLength(4);
            builder.Property(e => e.Tranuom).HasColumnName("tranuom").HasMaxLength(4);
            builder.Property(e => e.Mainuqty).HasColumnName("mainuqty").HasPrecision(14, 2);
            builder.Property(e => e.Subuqty).HasColumnName("subuqty").HasPrecision(14, 2);
            builder.Property(e => e.Tranuqty).HasColumnName("tranuqty").HasPrecision(14, 2);

            // Flags (smallint)
            builder.Property(e => e.Isbarcodestk).HasColumnName("isbarcodestk");
            builder.Property(e => e.Ispcs).HasColumnName("ispcs");
            builder.Property(e => e.Isweight).HasColumnName("isweight");
            builder.Property(e => e.Issingle).HasColumnName("issingle");
            builder.Property(e => e.Isshowweight).HasColumnName("isshowweight");
            builder.Property(e => e.Isstone).HasColumnName("isstone");
            builder.Property(e => e.Autopl).HasColumnName("autopl");
            builder.Property(e => e.Isscrap).HasColumnName("isscrap");
            builder.Property(e => e.Isbarcode).HasColumnName("isbarcode");
            builder.Property(e => e.Isratebypcs).HasColumnName("isratebypcs");
            builder.Property(e => e.Allowminus).HasColumnName("allowminus");

            // System & Audit
            builder.Property(e => e.Uniqueid).HasColumnName("uniqueid");
            builder.Property(e => e.Sdoctype).HasColumnName("sdoctype").HasMaxLength(6);
            builder.Property(e => e.Mdoctype).HasColumnName("mdoctype").HasMaxLength(5);
            builder.Property(e => e.Loccode).HasColumnName("loccode").HasMaxLength(55);
            builder.Property(e => e.Usercode).HasColumnName("usercode").HasMaxLength(55);
        }
    }
}
