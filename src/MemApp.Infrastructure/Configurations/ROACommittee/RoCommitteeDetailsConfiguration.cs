using MemApp.Domain.Entities.mem;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Res.Domain.Entities.RoaCommittee;

namespace Res.Infrastructure.Configurations.ROACommittee
{
    
    public class RoCommitteeDetailsConfiguration : IEntityTypeConfiguration<RoCommitteeDetail>
    {
        public void Configure(EntityTypeBuilder<RoCommitteeDetail> builder)
        {
            builder.ToTable("roa_CommitteeDetail");
            builder.HasKey(e => new { e.Id });
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(a => a.MemberName).IsRequired().HasMaxLength(250);



            builder.HasOne(k => k.Committee)
                 .WithMany(k => k.CommitteeDetails)
                 .HasForeignKey(k => new { k.CommitteeId })
                 .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
