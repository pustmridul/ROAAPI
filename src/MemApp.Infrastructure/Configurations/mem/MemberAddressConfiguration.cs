using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities;
using MemApp.Domain.Entities.mem;

namespace MemApp.Infrastructure.Configurations.mem;

public class MemberAddressConfiguration : IEntityTypeConfiguration<MemberAddress>
{
    public void Configure(EntityTypeBuilder<MemberAddress> builder)
    {
        builder.ToTable("mem_MemberAddress");
        builder.HasKey(e => new { e.Id });
        builder.Property(e=>e.Id).ValueGeneratedOnAdd();
        builder.Property(a=>a.Title).IsRequired().HasMaxLength(250);
    }
}
