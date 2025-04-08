using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities;
using MemApp.Domain.Entities.mem;

namespace MemApp.Infrastructure.Configurations.mem;

public class TableSetupConfiguration : IEntityTypeConfiguration<TableSetup>
{
    public void Configure(EntityTypeBuilder<TableSetup> builder)
    {
        builder.ToTable("mem_TableSetup");
        builder.HasKey(e => new { e.Id });
        builder.Property(e=>e.Id).ValueGeneratedOnAdd();
        builder.Property(a=>a.Title).IsRequired().HasMaxLength(250);
    }
}
