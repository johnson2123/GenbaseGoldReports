using GenbaseGoldReports.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenbaseGoldReports.Infrastructure.Persistence.Configurations.Master
{
    public class OtpLogConfiguration : IEntityTypeConfiguration<OtpLog>
    {
        public void Configure(EntityTypeBuilder<OtpLog> builder)
        {
            builder.ToTable("OtpLogs");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.Guid)
                .IsUnique();

            builder.Property(x => x.Target)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.OtpCode)
                .IsRequired()
                .HasMaxLength(6);

            builder.Property(x => x.Purpose)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.ActionToken)
                .HasMaxLength(256)
                .IsRequired(false);

            builder.Property(x => x.IPAddress)
                .HasMaxLength(50)
                .IsRequired(false);

            builder.Property(x => x.DeviceInfo)
                .HasMaxLength(256)
                .IsRequired(false);

            builder.HasOne(x => x.ApplicationUser)
                .WithMany(u => u.OtpLogs)
                .HasForeignKey(x => x.ApplicationUserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
