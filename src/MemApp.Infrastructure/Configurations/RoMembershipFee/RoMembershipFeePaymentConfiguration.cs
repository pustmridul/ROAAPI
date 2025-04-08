using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Res.Domain.Entities.ROAPayment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Res.Infrastructure.Configurations.RoMembershipFee
{
    
    public class RoMembershipFeePaymentConfiguration : IEntityTypeConfiguration<RoaMembershipFeePayment>
    {

        public void Configure(EntityTypeBuilder<RoaMembershipFeePayment> builder)
        {
            builder.ToTable("roa_MembershipFeePayment");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.MemberId).IsRequired();
            //  builder.Property(x => x.MemberPayment).IsRequired();
            //  builder.HasOne(x => x.Member).WithMany(y => y.SubscriptionPayments).HasForeignKey(z => z.MemberId).OnDelete(DeleteBehavior.NoAction);

        }

    }
}
