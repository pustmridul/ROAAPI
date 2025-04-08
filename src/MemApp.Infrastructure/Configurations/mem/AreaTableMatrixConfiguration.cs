using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities;
using MemApp.Domain.Entities.mem;

namespace MemApp.Infrastructure.Configurations.mem;

public class AreaTableMatrixConfiguration : IEntityTypeConfiguration<AreaTableMatrix>
{
    public void Configure(EntityTypeBuilder<AreaTableMatrix> builder)
    {
        builder.ToTable("mem_AreaTableMatrix");
        builder.HasKey(e => new { e.Id });
        builder.Property(e=>e.Id).ValueGeneratedOnAdd();
        builder.Property(a=>a.TableId).IsRequired();


        builder.HasOne(k => k.AreaLayout)
             .WithMany(k => k.AreaTableMatrices)
             .HasForeignKey(k => new { k.AreaLayoutId })
             .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(k => k.TableSetup)
             .WithMany(k => k.AreaTableMatrices)
             .HasForeignKey(k => new { k.TableId })
             .OnDelete(DeleteBehavior.NoAction);
    }
}
