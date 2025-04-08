using MemApp.Domain.Entities.subscription;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MemApp.Infrastructure.Configurations.subscription
{
    public class SubscriptionModConfiguration : IEntityTypeConfiguration<SubscriptionMode>
    {
        public void Configure(EntityTypeBuilder<SubscriptionMode> builder)
        {
            builder.ToTable("mem_SubscriptionMode");
            builder.HasKey(e => new { e.Id });
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(a => a.Name).IsRequired();
            builder.Property(a => a.Value).IsRequired();
        }
    }
}
