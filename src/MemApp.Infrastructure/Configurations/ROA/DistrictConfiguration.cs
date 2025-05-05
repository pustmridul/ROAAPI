using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities;
using MemApp.Domain.Entities.mem;
using Res.Domain.Entities;

namespace Res.Infrastructure.Configurations.ROA;

public class DistrictConfiguration : IEntityTypeConfiguration<District>
{
    public void Configure(EntityTypeBuilder<District> builder)
    {
        builder.ToTable("District_bck");
        builder.HasKey(e => new { e.Id });
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(a => a.EnglishName).IsRequired().HasMaxLength(100);

        // Configure foreign key relationship with District
        builder.HasOne(t => t.Division)
               .WithMany(d => d.Districts)
               .HasForeignKey(t => t.DivisionId)
               .OnDelete(DeleteBehavior.Cascade); // Deletes Thanas when District is deleted
    }
}
