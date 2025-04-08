using MemApp.Domain.Entities.services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Infrastructure.Configurations.service
{
    public class ServiceConfiguration: IEntityTypeConfiguration<ServiceRecord>
    {
        public void Configure(EntityTypeBuilder<ServiceRecord> builder)
        {
            builder.ToTable("ServiceRecord");
            builder.HasKey(e => new { e.Id });
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(a => a.Title).IsRequired().HasMaxLength(250);
        }
    }
}
