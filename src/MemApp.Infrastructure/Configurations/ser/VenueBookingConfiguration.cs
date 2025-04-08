using MemApp.Domain.Entities.ser;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MemApp.Infrastructure.Configurations.service
{
    public class VenueBookingConfiguration : IEntityTypeConfiguration<VenueBooking>
    {
        public void Configure(EntityTypeBuilder<VenueBooking> builder)
        {
            builder.ToTable("mem_VenueBooking");
            builder.HasKey(e => new { e.Id });
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
        }
    }


    public class VenueBookingDetailConfiguration : IEntityTypeConfiguration<VenueBookingDetail>
    {
        public void Configure(EntityTypeBuilder<VenueBookingDetail> builder)
        {
            builder.ToTable("mem_VenueBookingDetail");
            builder.HasKey(e => new { e.Id });
            builder.Property(e => e.Id).ValueGeneratedOnAdd();

            builder.HasOne(x => x.VenueBooking)
                .WithMany(y => y.VenueBookingDetails)
                .HasForeignKey(z => z.BookingId)
                .OnDelete(DeleteBehavior.NoAction);        
        }
    }
}
