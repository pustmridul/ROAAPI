using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities.com;

namespace MemApp.Infrastructure.Configurations.com;

public class ReleaseVersionConfiguration : IEntityTypeConfiguration<ReleaseVersion>
{
    public void Configure(EntityTypeBuilder<ReleaseVersion> builder)
    {
        builder.ToTable("mem_ReleaseVersion");
        builder.HasKey(e => new { e.Id });
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(a=>a.ReleaseTitle).IsRequired().HasMaxLength(50);
    }
}
