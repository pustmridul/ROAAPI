using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities;
using MemApp.Domain.Entities.mem;

namespace MemApp.Infrastructure.Configurations.mem;

public class CommitteeConfiguration : IEntityTypeConfiguration<Committee>
{
    public void Configure(EntityTypeBuilder<Committee> builder)
    {
        builder.ToTable("mem_Committee");
        builder.HasKey(e => new { e.Id });
        builder.Property(e=>e.Id).ValueGeneratedOnAdd();
      

        builder.HasOne(k => k.CommitteeCategory)
         .WithMany(k => k.Committees)
         .HasForeignKey(k => new { k.CommitteeCategoryId })
         .OnDelete(DeleteBehavior.NoAction);
    }
}
