using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities;
using MemApp.Domain.Entities.mem;
using Res.Domain.Entities;

namespace MemApp.Infrastructure.Configurations.mem;

public class MultipleOwnerConfiguration : IEntityTypeConfiguration<MultipleOwner>
{
    public void Configure(EntityTypeBuilder<MultipleOwner> builder)
    {
        builder.ToTable("roa_MultipleOwner");
        builder.HasKey(e => new { e.Id });
        builder.Property(e=>e.Id).ValueGeneratedOnAdd();
      //  builder.Property(a=>a.Email).IsRequired().HasMaxLength(100);
    }
}
