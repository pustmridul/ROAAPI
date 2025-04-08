using MemApp.Domain.Entities.ser;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MemApp.Infrastructure.Configurations.ser
{
    public class AvailabilityDetailConfiguration : IEntityTypeConfiguration<AvailabilityDetail>
    {
        public void Configure(EntityTypeBuilder<AvailabilityDetail> builder)
        {
            builder.ToTable("mem_AvailabilityDetail");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
           
            builder.HasOne(k => k.Availabilities)
                 .WithMany(k => k.AvailabilityDetails)
                 .HasForeignKey(k => new {k.AvailabilityId })
                 .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
