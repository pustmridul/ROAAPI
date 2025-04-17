using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities;
using MemApp.Domain.Entities.mem;
using Res.Domain.Entities;

namespace Res.Infrastructure.Configurations.ROA;

public class WardConfiguration : IEntityTypeConfiguration<Ward>
{
    public void Configure(EntityTypeBuilder<Ward> builder)
    {
        builder.ToTable("Ward");
        builder.HasKey(e => new { e.Id });
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(a => a.EnglishName).IsRequired().HasMaxLength(100);

        // Configure foreign key relationship with District
        builder.HasOne(t => t.Thana)
               .WithMany(d => d.Wards)
               .HasForeignKey(t => t.ThanaId)
               .OnDelete(DeleteBehavior.NoAction); // Deletes Thanas when District is deleted

        builder.HasOne(t => t.Municipality)
              .WithMany(d => d.Wards)
              .HasForeignKey(t => t.ThanaId)
              .OnDelete(DeleteBehavior.NoAction);
    }
}
