using MemApp.Domain.Entities.mem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MemApp.Infrastructure.Configurations.mem;

public class MessageInboxConfiguration : IEntityTypeConfiguration<MessageInbox>
{
    public void Configure(EntityTypeBuilder<MessageInbox> builder)
    {
        builder.ToTable("mem_MessageInbox");
        builder.HasKey(e => new { e.Id });
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
    }
}