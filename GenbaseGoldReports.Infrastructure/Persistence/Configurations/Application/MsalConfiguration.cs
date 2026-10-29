using GenbaseGoldReports.Domain.Entities.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenbaseGoldReports.Infrastructure.Persistence.Configurations.Application
{
    public class MsalConfiguration : IEntityTypeConfiguration<Msal>
    {
        public void Configure(EntityTypeBuilder<Msal> builder)
        {
            builder.ToTable("msal");

            builder.HasKey(x => new { x.Brncode, x.Divcode, x.Doctype, x.Docno });

            builder.Property(x => x.Brncode).HasColumnName("brncode").HasMaxLength(10);
            builder.Property(x => x.Divcode).HasColumnName("divcode").HasMaxLength(10);
            builder.Property(x => x.Doctype).HasColumnName("doctype").HasMaxLength(10);
            builder.Property(x => x.Docno).HasColumnName("DOCNO");

            builder.Property(x => x.Slno).HasColumnName("slno").HasMaxLength(14).ValueGeneratedNever();
            builder.Property(x => x.Mdoctype).HasColumnName("mdoctype").HasMaxLength(10);
            builder.Property(x => x.Sdoctype).HasColumnName("sdoctype").HasMaxLength(10);
            builder.Property(x => x.Docdate).HasColumnName("docdate");
            builder.Property(x => x.Stimestamp).HasColumnName("stimestamp").HasMaxLength(20);
            builder.Property(x => x.Loccode).HasColumnName("loccode").HasMaxLength(20);
            builder.Property(x => x.Usercode).HasColumnName("usercode").HasMaxLength(50);

            builder.Property(x => x.Partycode).HasColumnName("partycode").HasMaxLength(50);
            builder.Property(x => x.Partyname).HasColumnName("partyname").HasMaxLength(150);
            builder.Property(x => x.Partytrn).HasColumnName("partytrn").HasMaxLength(50);
            builder.Property(x => x.Addr1).HasColumnName("addr1").HasMaxLength(250);
            builder.Property(x => x.Sman).HasColumnName("sman").HasMaxLength(50);

            builder.Property(x => x.Mtltype).HasColumnName("mtltype").HasMaxLength(10);
            builder.Property(x => x.Ratetype).HasColumnName("ratetype").HasMaxLength(20);
            builder.Property(x => x.Gsrate).HasColumnName("gsrate").HasPrecision(18, 2);
            builder.Property(x => x.Gozrate).HasColumnName("gozrate").HasPrecision(18, 4);
            builder.Property(x => x.Usdrate).HasColumnName("usdrate").HasPrecision(18, 5);
            builder.Property(x => x.Ozfactor).HasColumnName("ozfactor").HasPrecision(18, 5);
            builder.Property(x => x.Gramrate).HasColumnName("gramrate").HasPrecision(18, 7);

            builder.Property(x => x.Curr).HasColumnName("curr").HasMaxLength(10);
            builder.Property(x => x.Curr1).HasColumnName("curr1").HasMaxLength(10);
            builder.Property(x => x.Currcode).HasColumnName("currcode").HasMaxLength(10);
            builder.Property(x => x.Currrate).HasColumnName("currrate").HasPrecision(18, 5);
            builder.Property(x => x.Lccurr).HasColumnName("lccurr").HasMaxLength(10);
            builder.Property(x => x.Basecurr).HasColumnName("basecurr").HasMaxLength(10);

            builder.Property(x => x.Fixunfix).HasColumnName("fixunfix").HasMaxLength(10);
            builder.Property(x => x.Impexptype).HasColumnName("impexptype").HasMaxLength(20);

            builder.Property(x => x.Totpcs).HasColumnName("totpcs").HasPrecision(18, 2);
            builder.Property(x => x.Pieces).HasColumnName("pieces").HasPrecision(18, 2);
            builder.Property(x => x.Totgrqty).HasColumnName("totgrqty").HasPrecision(18, 3);
            builder.Property(x => x.Totpuqty).HasColumnName("totpuqty").HasPrecision(18, 3);
            builder.Property(x => x.Totmkgamt).HasColumnName("totmkgamt").HasPrecision(18, 2);
            builder.Property(x => x.Totalmakingamount).HasColumnName("totalmakingamount").HasPrecision(18, 2);
            builder.Property(x => x.Totalmtlamt).HasColumnName("totalmtlamt").HasPrecision(18, 2);
            builder.Property(x => x.Totalmetalamount).HasColumnName("totalmetalamount").HasPrecision(18, 2);
            builder.Property(x => x.Tottaxamt).HasColumnName("tottaxamt").HasPrecision(18, 2);
            builder.Property(x => x.Totalvatamount).HasColumnName("totalvatamount").HasPrecision(18, 2);
            builder.Property(x => x.Totgrossamt).HasColumnName("totgrossamt").HasPrecision(18, 2);
            builder.Property(x => x.Gtotalamt).HasColumnName("gtotalamt").HasPrecision(18, 2);
            builder.Property(x => x.Totalamount).HasColumnName("totalamount").HasPrecision(18, 2);

            builder.Property(x => x.Goldamt).HasColumnName("goldamt").HasPrecision(18, 2);
            builder.Property(x => x.Makingvalue).HasColumnName("makingvalue").HasPrecision(18, 2);
            builder.Property(x => x.Totalvat).HasColumnName("totalvat").HasPrecision(18, 2);
            builder.Property(x => x.Totalpartyamt).HasColumnName("totalpartyamt").HasPrecision(18, 2);
            builder.Property(x => x.Totnetamtfc).HasColumnName("totnetamtfc").HasPrecision(18, 4);
            builder.Property(x => x.Totnetamtbc).HasColumnName("totnetamtbc").HasPrecision(18, 4);
            builder.Property(x => x.Stnamt).HasColumnName("stnamt").HasPrecision(18, 2);
            builder.Property(x => x.Totamtfc).HasColumnName("totamtfc").HasPrecision(18, 2);
            builder.Property(x => x.Amtpaid).HasColumnName("amtpaid").HasPrecision(18, 2);
            builder.Property(x => x.Totdedamtfc).HasColumnName("totdedamtfc").HasPrecision(18, 2);
            builder.Property(x => x.Totaddamtfc).HasColumnName("totaddamtfc").HasPrecision(18, 2);
            builder.Property(x => x.Refcharges).HasColumnName("refcharges").HasPrecision(18, 2);

            builder.Property(x => x.Premiumperc).HasColumnName("premiumperc").HasPrecision(18, 4);
            builder.Property(x => x.Premiumamount).HasColumnName("premiumamount").HasPrecision(18, 2);
            builder.Property(x => x.Discountperc).HasColumnName("discountperc").HasPrecision(18, 4);
            builder.Property(x => x.Discountamount).HasColumnName("discountamount").HasPrecision(18, 2);

            builder.Property(x => x.Partyamount).HasColumnName("partyamount").HasPrecision(18, 2);

            builder.Property(x => x.Ishedging).HasColumnName("ishedging");
            builder.Property(x => x.Autohedgingdocno).HasColumnName("autohedgingdocno");
            builder.Property(x => x.Mpydocno).HasColumnName("mpydocno");
            builder.Property(x => x.Refdocno).HasColumnName("refdocno").HasMaxLength(50);
            builder.Property(x => x.Refdocdate).HasColumnName("refdocdate");
            builder.Property(x => x.Narration).HasColumnName("narration").HasMaxLength(500);
            builder.Property(x => x.Hnarration).HasColumnName("hnarration").HasMaxLength(500);

            // One-to-Many Relationship with Msaldet
            builder.HasMany(x => x.Details)
                .WithOne()
                .HasForeignKey(d => new { d.Brncode, d.Divcode, d.Doctype, d.Docno })
                .IsRequired();
        }
    }
}
