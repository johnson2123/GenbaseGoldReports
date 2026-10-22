using GenbaseGoldReports.Domain.Entities.Navigation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenbaseGoldReports.Infrastructure.Persistence.Configurations.Master
{
    public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
    {
        public void Configure(EntityTypeBuilder<MenuItem> builder)
        {
            builder.ToTable("MenuItems");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.Path)
                .HasMaxLength(255);

            builder.Property(m => m.Icon)
                .HasMaxLength(50);

            builder.Property(m => m.DisplayOrder)
                .IsRequired();

            builder.Property(m => m.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(m => m.CreatedAtUtc)
                .IsRequired();

            builder.HasOne(m => m.Parent)
                .WithMany(m => m.Children)
                .HasForeignKey(m => m.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(m => m.RoleMenuItems)
                .WithOne(rm => rm.MenuItem)
                .HasForeignKey(rm => rm.MenuItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
