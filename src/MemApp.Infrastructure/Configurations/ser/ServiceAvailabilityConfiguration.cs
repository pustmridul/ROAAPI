using MemApp.Domain.Entities.ser;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MemApp.Infrastructure.Configurations.ser
{
    public class ServiceAvailabilityConfiguration : IEntityTypeConfiguration<ServiceAvailability>
    {
        public void Configure(EntityTypeBuilder<ServiceAvailability> builder)
        {
            builder.ToTable("mem_ServiceAvailability");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
           
        

        }
    }
}
