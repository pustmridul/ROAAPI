using MemApp.Domain.Entities.mem;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemApp.Domain.Entities;

namespace MemApp.Infrastructure.Configurations.mem
{
    public class LiquorPermitConfiguration:IEntityTypeConfiguration<LiquorPermit>
    {
        public void Configure(EntityTypeBuilder<LiquorPermit> builder)
        {
            builder.ToTable("mem_LiquorPermit");
            builder.HasKey(e => new { e.Id });
            builder.Property(e => e.Id).ValueGeneratedOnAdd()
            .HasMaxLength(250);

        }
    }
}
