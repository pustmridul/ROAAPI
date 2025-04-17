using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities;
using MemApp.Domain.Entities.mem;
using Res.Domain.Entities;

namespace Res.Infrastructure.Configurations.ROA;

public class MunicipalityConfiguration : IEntityTypeConfiguration<Municipality>
{
    public void Configure(EntityTypeBuilder<Municipality> builder)
    {
        builder.ToTable("Municipality");
        builder.HasKey(e => new { e.Id });
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(a => a.EnglishName).IsRequired().HasMaxLength(100);

        // Configure foreign key relationship with District
        builder.HasOne(t => t.Thana)
               .WithMany(d => d.Municipalities)
               .HasForeignKey(t => t.ThanaId)
               .OnDelete(DeleteBehavior.Cascade); // Deletes Thanas when District is deleted
    }
}
