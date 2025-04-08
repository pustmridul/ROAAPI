using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities;
using MemApp.Domain.Entities.mem;

namespace MemApp.Infrastructure.Configurations.mem;

public class MemberProfessionConfiguration : IEntityTypeConfiguration<MemberProfession>
{
    public void Configure(EntityTypeBuilder<MemberProfession> builder)
    {
        builder.ToTable("mem_MemberProfession");
        builder.HasKey(e => new { e.Id });
        builder.Property(e=>e.Id).ValueGeneratedOnAdd();
        builder.Property(a=>a.Name).IsRequired().HasMaxLength(20);
    }
}
