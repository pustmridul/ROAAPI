using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities.mem;

namespace MemApp.Infrastructure.Configurations.mem;

public class SlotMasterConfiguration : IEntityTypeConfiguration<SlotMaster>
{
    public void Configure(EntityTypeBuilder<SlotMaster> builder)
    {
        builder.ToTable("mem_SlotMaster");
        builder.HasKey(e => new { e.Id });
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
    }
}
