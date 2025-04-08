using MemApp.Domain.Entities.mem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MemApp.Infrastructure.Configurations.mem
{
    public class MemberFeesConfiguration : IEntityTypeConfiguration<MemberShipFee>
    {
        public void Configure(EntityTypeBuilder<MemberShipFee> builder)
        {
            builder.ToTable("mem_memberShipFees");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Title).IsRequired();
            builder.Property(x => x.Amount).IsRequired();

            builder.HasOne(x => x.MemberType)
                .WithMany(y => y.MemberShipFees)
                .HasForeignKey(z => z.MemberTypeId)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
