using MemApp.Domain.Entities.ser;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Infrastructure.Configurations.ser
{
    public class ServiceTicketAvailabilityConfiguration: IEntityTypeConfiguration<ServiceTicketAvailability>
    {
        public void Configure(EntityTypeBuilder<ServiceTicketAvailability> builder)
        {
            builder.ToTable("mem_ServiceTicketAvailability");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

        }
    }
}

