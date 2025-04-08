using MemApp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Infrastructure.Configurations.com
{
    public class UserRoleMapConfiguration : IEntityTypeConfiguration<UserRoleMap>
    {
        public void Configure(EntityTypeBuilder<UserRoleMap> builder)
        {
            builder.ToTable("com_UserRoleMap");
            builder.HasKey(e => new { e.Id });
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(k => k.RoleId).IsRequired();
            builder.Property(k => k.UserId).IsRequired();


            builder.HasOne(m => m.Role)
                  .WithMany(m => m.UserRoles)
                  .HasForeignKey(k => new { k.RoleId })
                  .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(m => m.User)
                .WithMany(m => m.UserRoles)
                .HasForeignKey(k => new { k.UserId })
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
