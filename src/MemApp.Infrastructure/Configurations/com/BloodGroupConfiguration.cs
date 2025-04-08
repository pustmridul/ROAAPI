using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities.com;

namespace MemApp.Infrastructure.Configurations.com;

public class BloodGroupConfiguration : IEntityTypeConfiguration<BloodGroup>
{
    public void Configure(EntityTypeBuilder<BloodGroup> builder)
    {
        builder.ToTable("mem_BloodGroup");
        builder.HasKey(e => new { e.Id });
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(a=>a.Code).IsRequired().HasMaxLength(20);
    }
}
