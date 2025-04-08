using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities;
namespace MemApp.Infrastructure.Configurations.com;

public class UserMenuMapConfiguration : IEntityTypeConfiguration<UserMenuMap>
{
    public void Configure(EntityTypeBuilder<UserMenuMap> builder)
    {
        builder.ToTable("com_UserMenuMap");
        builder.HasKey(e => new { e.Id });
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(a => a.UserId).IsRequired();
        builder.Property(a => a.MenuId).IsRequired();

        builder.HasOne(e => e.Menu)
            .WithMany(e=>e.UserMenuMaps)
            .HasForeignKey(e => e.MenuId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.User)
          .WithMany(e => e.UserMenuMaps)
          .HasForeignKey(e => e.UserId)
          .OnDelete(DeleteBehavior.NoAction);

      
    }
}
