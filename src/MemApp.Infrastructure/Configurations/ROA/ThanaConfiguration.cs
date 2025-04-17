using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities;
using MemApp.Domain.Entities.mem;
using Res.Domain.Entities;

namespace Res.Infrastructure.Configurations.ROA;

public class ThanaConfiguration : IEntityTypeConfiguration<Thana>
{
    public void Configure(EntityTypeBuilder<Thana> builder)
    {
        builder.ToTable("Thana");
        builder.HasKey(e => new { e.Id });
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(a => a.EnglishName).IsRequired().HasMaxLength(100);

        // Configure foreign key relationship with District
        builder.HasOne(t => t.District)
               .WithMany(d => d.Thanas)
               .HasForeignKey(t => t.DistrictId)
               .OnDelete(DeleteBehavior.Cascade); // Deletes Thanas when District is deleted
    }
}
