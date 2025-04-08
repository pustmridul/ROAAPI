using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities;
using MemApp.Domain.Entities.mem;

namespace MemApp.Infrastructure.Configurations.mem;

public class AreaLayoutConfiguration : IEntityTypeConfiguration<AreaLayout>
{
    public void Configure(EntityTypeBuilder<AreaLayout> builder)
    {
        builder.ToTable("mem_AreaLayout");
        builder.HasKey(e => new { e.Id });
        builder.Property(e=>e.Id).ValueGeneratedOnAdd();
        builder.Property(a=>a.Title).IsRequired().HasMaxLength(250);
    }
}
