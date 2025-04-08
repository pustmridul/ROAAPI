using MemApp.Domain.Entities.Communication;
using MemApp.Domain.Entities.Sale;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Infrastructure.Configurations.Communication
{
    public class NoticeConfiguration : IEntityTypeConfiguration<Notice>
    {
        public void Configure(EntityTypeBuilder<Notice> builder)
        {
            builder.ToTable("mem_Notice");
            builder.HasKey(e => new { e.Id });
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
        }
    }
}
