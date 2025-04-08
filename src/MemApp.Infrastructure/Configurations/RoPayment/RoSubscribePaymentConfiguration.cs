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
    public class RoSubscribePaymentConfiguration : IEntityTypeConfiguration<ROASubscriptionPayment>
    {
        
            public void Configure(EntityTypeBuilder<ROASubscriptionPayment> builder)
            {
                builder.ToTable("roa_SubscriptionPayment");
                builder.HasKey(x => x.Id);
                builder.Property(x => x.Id).ValueGeneratedOnAdd();
                builder.Property(x => x.MemberId).IsRequired();
              //  builder.Property(x => x.MemberPayment).IsRequired();
              //  builder.HasOne(x => x.Member).WithMany(y => y.SubscriptionPayments).HasForeignKey(z => z.MemberId).OnDelete(DeleteBehavior.NoAction);

            }
        
    }
}
