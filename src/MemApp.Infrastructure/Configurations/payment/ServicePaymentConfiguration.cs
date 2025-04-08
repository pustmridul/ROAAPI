using MemApp.Domain.Entities.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MemApp.Infrastructure.Configurations.payment
{
    public class ServicePaymentConfiguration : IEntityTypeConfiguration<SubscriptionPayment>
    {
        public void Configure(EntityTypeBuilder<SubscriptionPayment> builder)
        {
            builder.ToTable("mem_SubscriptionPayment");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.RegisterMemberId).IsRequired();
            builder.Property(x=>x.MemberPayment).IsRequired();
            builder.HasOne(x => x.RegisterMembers).WithMany(y => y.SubscriptionPayments).HasForeignKey(z => z.RegisterMemberId).OnDelete(DeleteBehavior.NoAction);

        }
    }
}
