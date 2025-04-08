using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities;
using MemApp.Domain.Entities.mem;

namespace MemApp.Infrastructure.Configurations.mem;

public class MemFilesConfiguration : IEntityTypeConfiguration<MemFiles>
{
    public void Configure(EntityTypeBuilder<MemFiles> builder)
    {
        builder.ToTable("mem_Files");
        builder.HasKey(e => new { e.Id });
        builder.Property(e=>e.Id).ValueGeneratedOnAdd();
    }
}
