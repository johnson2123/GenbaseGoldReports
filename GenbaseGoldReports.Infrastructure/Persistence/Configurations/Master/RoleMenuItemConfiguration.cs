using GenbaseGoldReports.Domain.Entities.Navigation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenbaseGoldReports.Infrastructure.Persistence.Configurations.Master
{
    public class RoleMenuItemConfiguration : IEntityTypeConfiguration<RoleMenuItem>
    {
        public void Configure(EntityTypeBuilder<RoleMenuItem> builder)
        {
            builder.ToTable("RoleMenuItems");

            builder.HasKey(rm => rm.Id);

            builder.Property(rm => rm.RoleName)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(rm => new { rm.RoleName, rm.MenuItemId })
                .IsUnique();
        }
    }
}
