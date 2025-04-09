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
   
    public class RoCommitteeCategoryConfiguration : IEntityTypeConfiguration<RoCommitteeCategory>
    {
        public void Configure(EntityTypeBuilder<RoCommitteeCategory> builder)
        {
            builder.ToTable("roa_CommitteeCategory");
            builder.HasKey(e => new { e.Id });
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
        }
    }
}
