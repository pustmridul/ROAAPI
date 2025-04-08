using MemApp.Domain.Entities.subscription;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Res.Domain.Entities.ROASubscription;

namespace Res.Infrastructure.Configurations.RoSubscription
{
    public class ROASubscriptionFeeConfiguration : IEntityTypeConfiguration<ROASubscriptionFee>
    {
      
            public void Configure(EntityTypeBuilder<ROASubscriptionFee> builder)
            {
                builder.ToTable("roa_SubscriptionFee");
                builder.HasKey(e => new { e.Id });
                builder.Property(e => e.Id).ValueGeneratedOnAdd();
                builder.Property(a => a.SubscribedYear).IsRequired();
              //  builder.Property(a => a.SubscriptionModId).IsRequired();
                builder.Property(a => a.SubscriptionFee).IsRequired();
                builder.Property(a => a.LateFee).IsRequired();
              //  builder.Property(a => a.AbroadFee).IsRequired();

                //builder.HasOne(x => x.SubscriptionMode)
                //    .WithMany(y => y.SubscriptionFees)
                //    .HasForeignKey(z => z.SubscriptionModId)
                //    .OnDelete(DeleteBehavior.NoAction);
            }
        
    }
}
