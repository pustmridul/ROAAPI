using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities;
using MemApp.Domain.Entities.com;

namespace MemApp.Infrastructure.Configurations.com;

public class TramsAndConditionConfiguration : IEntityTypeConfiguration<TramsAndCondition>
{
    public void Configure(EntityTypeBuilder<TramsAndCondition> builder)
    {
        builder.ToTable("mem_TramsAndCondition");
        builder.HasKey(e => new { e.Id });
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
    }
}
