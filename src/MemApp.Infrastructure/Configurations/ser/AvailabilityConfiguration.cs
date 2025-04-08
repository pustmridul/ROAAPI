using MemApp.Domain.Entities.mem;
using MemApp.Domain.Entities.ser;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MemApp.Infrastructure.Configurations.ser
{
    public class AvailabilityConfiguration : IEntityTypeConfiguration<Availability>
    {
        public void Configure(EntityTypeBuilder<Availability> builder)
        {
            builder.ToTable("mem_Availability");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
           
           

        }
    }
}
