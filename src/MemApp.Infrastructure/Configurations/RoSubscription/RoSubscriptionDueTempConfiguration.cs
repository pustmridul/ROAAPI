using MemApp.Domain.Entities.Payment;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Res.Domain.Entities.ROAPayment;

namespace Res.Infrastructure.Configurations.RoSubscription
{
    public class RoSubscriptionDueTempConfiguration : IEntityTypeConfiguration<RoSubscriptionDueTemp>
    {
       
            public void Configure(EntityTypeBuilder<RoSubscriptionDueTemp> builder)
            {
                builder.ToTable("roa_SubscriptionDueTemp");
                builder.HasKey(k => k.Id);

            }
        
    }
}
