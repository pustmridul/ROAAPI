using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities;
using MemApp.Domain.Entities.mem;
using Res.Domain.Entities;

namespace MemApp.Infrastructure.Configurations.mem;

public class RoaMemberCatConfiguration : IEntityTypeConfiguration<RoaMemberCategory>
{
    public void Configure(EntityTypeBuilder<RoaMemberCategory> builder)
    {
        builder.ToTable("roa_MemberCategory");
        builder.HasKey(e => new { e.Id });
        builder.Property(e=>e.Id).ValueGeneratedOnAdd();
      //  builder.Property(a=>a.EnglishName).IsRequired().HasMaxLength(100);

        // Configure foreign key relationship with District
        //builder.HasOne(t => t.District)
        //       .WithMany(d => d.Zones)
        //       .HasForeignKey(t => t.DistrictId)
        //       .OnDelete(DeleteBehavior.NoAction); // Deletes Thanas when District is deleted
    }
}
