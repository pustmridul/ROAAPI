using MemApp.Domain.Entities.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MemApp.Infrastructure.Configurations.payment
{
    public class ServicePaymentDetailConfiguration : IEntityTypeConfiguration<SubscriptionPaymentDetail>
    {
        public void Configure(EntityTypeBuilder<SubscriptionPaymentDetail> builder)
        {
            builder.ToTable("mem_SubscriptionPaymentDetail");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            
            builder.HasOne(x => x.SubscriptionFees)
                .WithMany(y => y.SubscriptionPaymentDetails)
                .HasForeignKey(z => z.SubscriptionFeesId)
                .OnDelete(DeleteBehavior.NoAction);
            
            builder.HasOne(x => x.SubscriptionPayments)
                .WithMany(y => y.SubscriptionPaymentDetails)
                .HasForeignKey(z => z.SubscriptionPaymentId)
                .OnDelete(DeleteBehavior.NoAction);
            
            builder.HasOne(x => x.RegisterMembers)
                .WithMany(y => y.SubscriptionPaymentDetails)
                .HasForeignKey(z => z.RegisterMemberId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
