using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities.mem;

namespace MemApp.Infrastructure.Configurations.mem;

public class CommitteeDetailsConfiguration : IEntityTypeConfiguration<CommitteeDetail>
{
    public void Configure(EntityTypeBuilder<CommitteeDetail> builder)
    {
        builder.ToTable("mem_CommitteeDetail");
        builder.HasKey(e => new { e.Id });
        builder.Property(e=>e.Id).ValueGeneratedOnAdd();
        builder.Property(a=>a.MemberName).IsRequired().HasMaxLength(250);



        builder.HasOne(k => k.Committee)
             .WithMany(k => k.CommitteeDetails)
             .HasForeignKey(k => new { k.CommitteeId })
             .OnDelete(DeleteBehavior.NoAction);

    }
}
