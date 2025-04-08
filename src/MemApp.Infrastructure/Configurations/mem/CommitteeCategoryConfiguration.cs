using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities.mem;

namespace MemApp.Infrastructure.Configurations.mem;

public class CommitteeCategoryConfiguration : IEntityTypeConfiguration<CommitteeCategory>
{
    public void Configure(EntityTypeBuilder<CommitteeCategory> builder)
    {
        builder.ToTable("mem_CommitteeCategory");
        builder.HasKey(e => new { e.Id });
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
    }
}
