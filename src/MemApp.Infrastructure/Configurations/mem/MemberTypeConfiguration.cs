using MemApp.Domain.Entities.mem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MemApp.Infrastructure.Configurations.mem;

public class MemberTypeConfiguration : IEntityTypeConfiguration<MemberType>
{
    public void Configure(EntityTypeBuilder<MemberType> builder)
    {
        builder.ToTable("mem_MemberType");
        builder.HasKey(e => new { e.Id });
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(a => a.Name).IsRequired().HasMaxLength(20);
        builder.HasOne(x => x.CategoryPatterns).WithMany(y => y.MemberTypes).HasForeignKey(z => z.CategoryPatternId).OnDelete(DeleteBehavior.NoAction);
    }
}
