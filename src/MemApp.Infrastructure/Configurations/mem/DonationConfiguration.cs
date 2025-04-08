using MemApp.Domain.Entities.mem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MemApp.Infrastructure.Configurations.mem;

public class DonationConfiguration : IEntityTypeConfiguration<Donation>
{
    public void Configure(EntityTypeBuilder<Donation> builder)
    {
        builder.ToTable("mem_Donation");
        builder.HasKey(e => new { e.Id });
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(a => a.Title).IsRequired().HasMaxLength(500);

        builder.HasQueryFilter(q => q.IsActive);
    }
}
