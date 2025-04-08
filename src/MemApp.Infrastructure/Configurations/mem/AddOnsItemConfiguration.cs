using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities;
using MemApp.Domain.Entities.mem;

namespace MemApp.Infrastructure.Configurations.mem;

public class AddOnsItemConfiguration : IEntityTypeConfiguration<AddOnsItem>
{
    public void Configure(EntityTypeBuilder<AddOnsItem> builder)
    {
        builder.ToTable("mem_AddOnsItem");
        builder.HasKey(e => new { e.Id });
        builder.Property(e=>e.Id).ValueGeneratedOnAdd();
        builder.Property(a=>a.Title).IsRequired().HasMaxLength(500);
        builder.Property(a => a.Price).IsRequired();

       
    }
}
