using MemApp.Domain.Entities.Payment;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Res.Domain.Entities.ROAPayment;

namespace Res.Infrastructure.Configurations.RoPayment
{
    public class RoSubscribePaymentDetailConfiguration : IEntityTypeConfiguration<ROASubscriptionPaymentDetail>
    {
       
            public void Configure(EntityTypeBuilder<ROASubscriptionPaymentDetail> builder)
            {
                builder.ToTable("roa_SubscriptionPaymentDetail");
                builder.HasKey(x => x.Id);
                builder.Property(x => x.Id).ValueGeneratedOnAdd();

                //builder.HasOne(x => x.SubscriptionFee)
                //    .WithMany(y => y.SubscriptionPaymentDetails)
                //    .HasForeignKey(z => z.SubscriptionFeesId)
                //    .OnDelete(DeleteBehavior.NoAction);

                builder.HasOne(x => x.SubscriptionPayment)
                    .WithMany(y => y.SubscriptionPaymentDetails)
                    .HasForeignKey(z => z.SubscriptionPaymentId)
                    .OnDelete(DeleteBehavior.NoAction);

                //builder.HasOne(x => x.Member)
                //    .WithMany(y => y.SubscriptionPaymentDetails)
                //    .HasForeignKey(z => z.MemberInfoId)
                //    .OnDelete(DeleteBehavior.NoAction);
            }
        
    }
}
