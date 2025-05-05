using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities;
using MemApp.Domain.Entities.mem;
using Res.Domain.Entities;

namespace MemApp.Infrastructure.Configurations.mem;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");
        builder.HasKey(e => new { e.Id });
        builder.Property(e=>e.Id).ValueGeneratedOnAdd();
        builder.Property(a=>a.OperationName).IsRequired().HasMaxLength(100);

        // Configure foreign key relationship with District
        //builder.HasOne(t => t.District)
        //       .WithMany(d => d.Zones)
        //       .HasForeignKey(t => t.DistrictId)
        //       .OnDelete(DeleteBehavior.NoAction); // Deletes Thanas when District is deleted
    }
}
