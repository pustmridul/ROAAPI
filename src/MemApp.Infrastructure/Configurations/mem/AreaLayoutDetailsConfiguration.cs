using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities.mem;

namespace MemApp.Infrastructure.Configurations.mem;

public class AreaLayoutDetailsConfiguration : IEntityTypeConfiguration<AreaLayoutDetail>
{
    public void Configure(EntityTypeBuilder<AreaLayoutDetail> builder)
    {
        builder.ToTable("mem_AreaLayoutDetail");
        builder.HasKey(e => new { e.Id });
        builder.Property(e=>e.Id).ValueGeneratedOnAdd();
        builder.Property(a=>a.TableName).IsRequired().HasMaxLength(250);



        builder.HasOne(k => k.AreaLayout)
             .WithMany(k => k.AreaLayoutDetails)
             .HasForeignKey(k => new { k.AreaLayoutId })
             .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(k=>k.TableSetup)
            .WithMany(k=>k.AreaLayoutDetails)
            .HasForeignKey(k=>new {k.TableId})
            .OnDelete(DeleteBehavior.NoAction);

    }
}
