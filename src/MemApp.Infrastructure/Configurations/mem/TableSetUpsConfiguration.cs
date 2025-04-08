using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities;
using MemApp.Domain.Entities.mem;
using MemApp.Domain.Entities.com;

namespace MemApp.Infrastructure.Configurations.mem;

public class BoardMeetingMinuetsConfiguration : IEntityTypeConfiguration<BoardMeetingMinuet>
{
    public void Configure(EntityTypeBuilder<BoardMeetingMinuet> builder)
    {
        builder.ToTable("mem_BoardMeetingMinuet");
        builder.HasKey(e => new { e.Id });
        builder.Property(e=>e.Id).ValueGeneratedOnAdd();
    }
}
