using GenbaseGoldReports.Domain.Entities.Masters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenbaseGoldReports.Infrastructure.Persistence.Configurations.Application
{
    public class ActypesConfiguration : IEntityTypeConfiguration<Actypes>
    {
        public void Configure(EntityTypeBuilder<Actypes> builder)
        {
            builder.ToTable("actypes");

            builder.HasKey(e => e.Code);
            builder.Property(e => e.Code)
                .HasColumnName("code")
                .HasMaxLength(5)
                .ValueGeneratedNever();

            builder.Property(e => e.Descript)
                .HasColumnName("descript")
                .HasMaxLength(50);

            builder.Property(e => e.Loccode)
                .HasColumnName("loccode");

            builder.Property(e => e.Usercode)
                .HasColumnName("usercode");
        }
    }
}
