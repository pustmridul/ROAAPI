using MemApp.Domain.Entities.Payment;
using MemApp.Domain.Entities.subscription;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MemApp.Infrastructure.Configurations.subscription
{
    public class SubscriptionPaymentTempConfiguration : IEntityTypeConfiguration<SubscriptionPaymentTemp>
    {
        public void Configure(EntityTypeBuilder<SubscriptionPaymentTemp> builder)
        {
            builder.ToTable("mem_SubscriptionPaymentTemp");
            builder.HasKey(e => new { e.Id });
            builder.Property(e => e.Id).ValueGeneratedOnAdd();

           
            builder.HasOne(x => x.RegisterMember)
               .WithMany(y => y.SubscriptionPaymentTemp)
               .HasForeignKey(z => z.RegisterMemberId)
               .OnDelete(DeleteBehavior.NoAction);

        }
    }

    public class SubscriptionDueTempConfiguration : IEntityTypeConfiguration<SubscriptionDueTemp>
    {
        public void Configure(EntityTypeBuilder<SubscriptionDueTemp> builder)
        {
            builder.ToTable("mem_SubscriptionDueTemp");
            builder.HasKey(k=>k.Id);

        }
    }
}
