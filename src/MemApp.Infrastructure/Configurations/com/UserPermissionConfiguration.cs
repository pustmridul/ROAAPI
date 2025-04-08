using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities;

namespace MemApp.Infrastructure.Configurations.com;

public class UserPermissionConfiguration : IEntityTypeConfiguration<UserPermission>
{
    public void Configure(EntityTypeBuilder<UserPermission> builder)
    {
        builder.ToTable("com_UserPermission");
        builder.HasKey(e => new { e.Id });
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e=>e.UserId).IsRequired();
        builder.Property(e => e.PermissionNo).IsRequired();
    }
}
