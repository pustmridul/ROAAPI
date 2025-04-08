using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities;
using MemApp.Domain.Entities.mem;

namespace MemApp.Infrastructure.Configurations.mem;

public class ServiceSaleAvailabilityConfiguration : IEntityTypeConfiguration<ServiceSaleAvailability>
{
    public void Configure(EntityTypeBuilder<ServiceSaleAvailability> builder)
    {
        builder.ToTable("mem_ServiceSaleAvailability");
        builder.HasKey(e => new { e.Id });
        builder.Property(e=>e.Id).ValueGeneratedOnAdd();
        //builder.Property(a=>a.Name).IsRequired().HasMaxLength(250);

        builder.HasOne(x => x.ServiceSale);
            
    }
}
