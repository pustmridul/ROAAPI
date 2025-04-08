using MemApp.Domain.Entities.subscription;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MemApp.Infrastructure.Configurations.subscription
{
    public class SubscriptionFeesConfiguration : IEntityTypeConfiguration<SubscriptionFees>
    {
        public void Configure(EntityTypeBuilder<SubscriptionFees> builder)
        {
            builder.ToTable("mem_SubscriptionFees");
            builder.HasKey(e => new { e.Id });
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(a => a.SubscribedYear).IsRequired();
            builder.Property(a => a.SubscriptionModId).IsRequired();
            builder.Property(a => a.SubscriptionFee).IsRequired();
            builder.Property(a => a.LateFee).IsRequired();
            builder.Property(a => a.AbroadFee).IsRequired();

            builder.HasOne(x => x.SubscriptionMode)
                .WithMany(y => y.SubscriptionFees)
                .HasForeignKey(z => z.SubscriptionModId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
