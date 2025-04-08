using MemApp.Domain.Entities.mem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MemApp.Infrastructure.Configurations.com
{
    public class MemberFeesMapConfiguration : IEntityTypeConfiguration<MemberFeesMap>
    {
        public void Configure(EntityTypeBuilder<MemberFeesMap> builder)
        {
            builder.ToTable("mem_MemberFeesMap");
            builder.HasKey(e => new { e.Id });
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(a => a.RegisterMemberId).IsRequired();
            builder.Property(a => a.MemberShipFeeId).IsRequired();

            builder.HasOne(e => e.RegisterMember)
                .WithMany(e => e.MemberFeesMaps)
                .HasForeignKey(e => e.RegisterMemberId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(e => e.MemberShipFee)
              .WithMany(e => e.MemberFeesMaps)
              .HasForeignKey(e => e.MemberShipFeeId)
              .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
