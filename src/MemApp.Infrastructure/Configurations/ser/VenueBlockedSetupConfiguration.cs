using MemApp.Domain.Entities.mem;
using MemApp.Domain.Entities.ser;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MemApp.Infrastructure.Configurations.ser
{
    public class VenueBlockedSetupConfiguration : IEntityTypeConfiguration<VenueBlockedSetup>
    {
        public void Configure(EntityTypeBuilder<VenueBlockedSetup> builder)
        {
            builder.ToTable("mem_VenueBlockedSetup");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
           
           

        }
    }
}
