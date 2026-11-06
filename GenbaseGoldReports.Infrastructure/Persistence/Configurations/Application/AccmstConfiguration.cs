using GenbaseGoldReports.Domain.Entities.Masters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenbaseGoldReports.Infrastructure.Persistence.Configurations.Application
{
    public class AccmstConfiguration : IEntityTypeConfiguration<Accmst>
    {
        public void Configure(EntityTypeBuilder<Accmst> builder)
        {
            builder.ToTable("accmst");

            builder.HasKey(e => e.Code);
            builder.Property(e => e.Code).HasColumnName("code").HasMaxLength(6).ValueGeneratedNever();

            builder.Property(e => e.Grpcode).HasColumnName("grpcode").HasMaxLength(6);
            builder.Property(e => e.Acname).HasColumnName("acname").HasMaxLength(255);
            builder.Property(e => e.Accode).HasColumnName("accode").HasMaxLength(6);
            builder.Property(e => e.Grpname).HasColumnName("grpname").HasMaxLength(55);

            builder.Property(e => e.Isacc).HasColumnName("isacc");
            builder.Property(e => e.Levelid).HasColumnName("levelid");
            builder.Property(e => e.Keycode).HasColumnName("keycode").HasPrecision(16, 4);

            builder.Property(e => e.Actype).HasColumnName("actype").HasMaxLength(1);
            builder.Property(e => e.Class).HasColumnName("class").HasMaxLength(3);
            builder.Property(e => e.Brncode).HasColumnName("brncode").HasMaxLength(2);

            builder.Property(e => e.Curr).HasColumnName("curr").HasMaxLength(3);
            builder.Property(e => e.Currrate).HasColumnName("currrate").HasPrecision(14, 5);

            builder.Property(e => e.Trnno).HasColumnName("trnno").HasMaxLength(55);
            builder.Property(e => e.Taxrate).HasColumnName("taxrate").HasPrecision(14, 2);
            builder.Property(e => e.Isvatgroup).HasColumnName("isvatgroup");

            builder.Property(e => e.Addr1).HasColumnName("addr1").HasMaxLength(55);
            builder.Property(e => e.Addr2).HasColumnName("addr2").HasMaxLength(55);
            builder.Property(e => e.Addr3).HasColumnName("addr3").HasMaxLength(55);
            builder.Property(e => e.Phone).HasColumnName("phone").HasMaxLength(55);
            builder.Property(e => e.Mobile).HasColumnName("mobile").HasMaxLength(55);
            builder.Property(e => e.Contactperson).HasColumnName("contactperson").HasMaxLength(55);
            builder.Property(e => e.Email).HasColumnName("email").HasMaxLength(55);
            builder.Property(e => e.Website).HasColumnName("website").HasMaxLength(55);
            builder.Property(e => e.City).HasColumnName("city").HasMaxLength(55);
            builder.Property(e => e.Country).HasColumnName("country").HasMaxLength(55);
            builder.Property(e => e.Area).HasColumnName("area").HasMaxLength(6);

            builder.Property(e => e.Licencenumber).HasColumnName("licencenumber").HasMaxLength(55);
            builder.Property(e => e.Licencedate).HasColumnName("licencedate").HasMaxLength(55);

            builder.Property(e => e.Selected).HasColumnName("selected");
            builder.Property(e => e.Docdate).HasColumnName("docdate");
            builder.Property(e => e.Purusdrate).HasColumnName("purusdrate").HasPrecision(14, 2);
            builder.Property(e => e.Salusdrate).HasColumnName("salusdrate").HasPrecision(14, 2);
            builder.Property(e => e.Marginperc).HasColumnName("marginperc").HasPrecision(14, 2);
            builder.Property(e => e.Marginconsumeperc).HasColumnName("marginconsumeperc").HasPrecision(14, 2);
            builder.Property(e => e.Loccode).HasColumnName("loccode").HasMaxLength(55);
            builder.Property(e => e.Usercode).HasColumnName("usercode").HasMaxLength(55);

            // --- Hardened Legacy Sequence & Audit Mappings ---
            builder.Property(e => e.Docno).HasColumnName("DOCNO");
            builder.Property(e => e.Mrtype).HasColumnName("mrtype").HasMaxLength(3);
            builder.Property(e => e.Srno).HasColumnName("SRNO");
            builder.Property(e => e.Mstsrno).HasColumnName("mstsrno").HasMaxLength(10);
            builder.Property(e => e.Doctype).HasColumnName("doctype").HasMaxLength(6);
            builder.Property(e => e.Slno).HasColumnName("slno").HasMaxLength(6);
            builder.Property(e => e.Parcode).HasColumnName("parcode").HasMaxLength(6);
            builder.Property(e => e.Maingroup).HasColumnName("maingroup");
            builder.Property(e => e.Mdoctype).HasColumnName("mdoctype").HasMaxLength(3);
            builder.Property(e => e.Sdoctype).HasColumnName("sdoctype").HasMaxLength(4);
            builder.Property(e => e.Bnkcode).HasColumnName("bnkcode").HasMaxLength(6);
            builder.Property(e => e.Bnkactype).HasColumnName("bnkactype").HasMaxLength(2);
            builder.Property(e => e.Bnkacno).HasColumnName("bnkacno").HasMaxLength(55);
            builder.Property(e => e.Mobileno).HasColumnName("mobileno").HasMaxLength(55);
            builder.Property(e => e.Isconfacc).HasColumnName("isconfacc");
            builder.Property(e => e.Cvrno).HasColumnName("cvrno").HasMaxLength(6);
            builder.Property(e => e.Isspecialacc).HasColumnName("isspecialacc").HasMaxLength(1);
            builder.Property(e => e.Custcat).HasColumnName("custcat").HasMaxLength(4);
            builder.Property(e => e.Custtype).HasColumnName("custtype").HasMaxLength(4);
            builder.Property(e => e.Isalloc).HasColumnName("isalloc");
            builder.Property(e => e.Purgozrate).HasColumnName("purgozrate").HasPrecision(14, 2);
            builder.Property(e => e.Salgozrate).HasColumnName("salgozrate").HasPrecision(14, 2);
            builder.Property(e => e.Purconvrate).HasColumnName("purconvrate").HasPrecision(14, 2);
            builder.Property(e => e.Salconvrate).HasColumnName("salconvrate").HasPrecision(14, 2);
            builder.Property(e => e.Purtxntype).HasColumnName("purtxntype").HasMaxLength(55);
            builder.Property(e => e.Saltxntype).HasColumnName("saltxntype").HasMaxLength(55);
            builder.Property(e => e.Custarea).HasColumnName("custarea").HasMaxLength(4);
            builder.Property(e => e.Sharepercentage).HasColumnName("sharepercentage").HasPrecision(14, 2);
        }
    }
}
