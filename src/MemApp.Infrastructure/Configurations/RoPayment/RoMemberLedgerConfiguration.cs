using MemApp.Domain.Entities.Payment;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Res.Domain.Entities.ROAPayment;
using Res.Domain.Entities;

namespace Res.Infrastructure.Configurations.RoPayment
{
    public class RoMemberLedgerConfiguration : IEntityTypeConfiguration<RoMemberLedger>
    {
        
            public void Configure(EntityTypeBuilder<RoMemberLedger> builder)
            {
                builder.ToTable("roa_MemberLedger");
           
                builder.HasKey(e => new { e.MemberLedgerID });
                builder.Property(e => e.MemberLedgerID).ValueGeneratedOnAdd();
                builder.Property(e => e.MemberLedgerID).HasColumnType("numeric(18, 0)");
                builder.Property(e => e.Amount).HasColumnType("decimal(18,2)");

                builder.Property(a => a.MemberId).IsRequired().HasMaxLength(10);

        }
        
    }
}
