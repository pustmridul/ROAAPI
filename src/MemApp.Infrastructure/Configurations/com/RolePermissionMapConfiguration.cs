using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities.com;

namespace MemApp.Infrastructure.Configurations.com;

public class RolePermissionMapConfiguration : IEntityTypeConfiguration<RolePermissionMap>
{
    public void Configure(EntityTypeBuilder<RolePermissionMap> builder)
    {
        builder.ToTable("com_RolePermissionMap");
        builder.HasKey(e => new { e.Id });
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(a=>a.RoleId).IsRequired();
        builder.Property(a => a.PermissionNo).IsRequired();

    }
}
