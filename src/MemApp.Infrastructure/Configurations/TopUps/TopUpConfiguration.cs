using MemApp.Domain.Entities.com;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MemApp.Infrastructure.Configurations.TopUps
{
    public class TopUpConfiguration : IEntityTypeConfiguration<TopUp>
    {
        public void Configure(EntityTypeBuilder<TopUp> builder)
        {
            builder.ToTable("mem_TopUp");
            builder.HasKey(e => new { e.Id });
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(a => a.MemberShipNo).IsRequired();
            builder.Property(a => a.Status).IsRequired();

            //builder.HasOne(x=>x.RegisterMember)
            //    .WithMany(y=>y.TopUps)
            //    .HasForeignKey(z=>z.RegisterMemberId)
            //    .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.MemberRegistrationInfo)
               .WithMany(y => y.TopUps)
               .HasForeignKey(z => z.MemberId)
               .OnDelete(DeleteBehavior.NoAction);

        }
    }

    public class TopUpDetailConfiguration : IEntityTypeConfiguration<TopUpDetail>
    {
        public void Configure(EntityTypeBuilder<TopUpDetail> builder)
        {
            builder.ToTable("mem_TopUpDetail");
            builder.HasKey(e => new { e.Id });
            builder.Property(e => e.Id).ValueGeneratedOnAdd();


            builder.HasOne(x => x.TopUp)
                .WithMany(y => y.TopUpDetails)
                .HasForeignKey(z => z.TopUpId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.PaymentMethod)
                .WithMany(y => y.TopUpDetails)
                .HasForeignKey(k => k.PaymentMethodId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
