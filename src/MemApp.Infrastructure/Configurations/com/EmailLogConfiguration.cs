using MemApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Infrastructure.Configurations.com
{
    public class EmailLogConfiguration : IEntityTypeConfiguration<EmailLog>
    {
        public void Configure(EntityTypeBuilder<EmailLog> builder)
        {
            builder.ToTable("mem_EmailLog");
            builder.HasKey(e => new { e.Id });
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
        }
    }
    
}
