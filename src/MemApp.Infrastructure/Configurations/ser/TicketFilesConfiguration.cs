using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities.ser;

namespace MemApp.Infrastructure.Configurations.ser;

public class TicketFilesConfiguration : IEntityTypeConfiguration<TicketFiles>
{
    public void Configure(EntityTypeBuilder<TicketFiles> builder)
    {
        builder.ToTable("mem_TicketFiles");
        builder.HasKey(e => new { e.Id });
        builder.Property(e=>e.Id).ValueGeneratedOnAdd();
    }
}
