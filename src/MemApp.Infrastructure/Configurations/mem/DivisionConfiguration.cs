using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities;
using MemApp.Domain.Entities.mem;
using Res.Domain.Entities;

namespace MemApp.Infrastructure.Configurations.mem;

public class DivisionConfiguration : IEntityTypeConfiguration<Division>
{
    public void Configure(EntityTypeBuilder<Division> builder)
    {
        builder.ToTable("Division");
        builder.HasKey(e => new { e.Id });
        builder.Property(e=>e.Id).ValueGeneratedOnAdd();
        builder.Property(a=>a.EnglishName).IsRequired().HasMaxLength(100);
    }
}
