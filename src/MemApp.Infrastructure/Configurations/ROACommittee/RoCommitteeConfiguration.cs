using MemApp.Domain.Entities.mem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Res.Domain.Entities.RoaCommittee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Res.Infrastructure.Configurations.ROACommittee
{
    public class RoCommitteeConfiguration : IEntityTypeConfiguration<RoCommittee>
    {
        public void Configure(EntityTypeBuilder<RoCommittee> builder)
        {
            builder.ToTable("roa_Committee");
            builder.HasKey(e => new { e.Id });
            builder.Property(e => e.Id).ValueGeneratedOnAdd();


            builder.HasOne(k => k.CommitteeCategory)
             .WithMany(k => k.Committees)
             .HasForeignKey(k => new { k.CommitteeCategoryId })
             .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
