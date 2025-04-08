using MemApp.Domain.Entities.mem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MemApp.Infrastructure.Configurations.mem
{
    public class MemberActiveStatusConfiguration : IEntityTypeConfiguration<MemberActiveStatus>
    {
        public void Configure(EntityTypeBuilder<MemberActiveStatus> builder)
        {
            builder.ToTable("mem_MemberActiveStatus");
            builder.HasKey(e => new { e.Id });
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(a => a.Name).IsRequired().HasMaxLength(20);
        }
    }
}
