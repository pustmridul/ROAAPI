using MemApp.Domain.Entities.mem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MemApp.Infrastructure.Configurations.mem
{
    public class MemberChildrenConfiguration:IEntityTypeConfiguration<MemberChildren>
    {
        public void Configure(EntityTypeBuilder<MemberChildren> builder)
        {
            builder.ToTable("mem_memberChildren");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
          
            builder.HasOne(x => x.RegisterMembers)
                .WithMany(y => y.MemberChildrens)
                .HasForeignKey(z => z.RegisterMemberId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.MemberTemp)
                .WithMany(y => y.MemberChildrens)
                .HasForeignKey(z => z.RegisterMemberId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
