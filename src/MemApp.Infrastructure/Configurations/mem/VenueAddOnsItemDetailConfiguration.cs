using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities;
using MemApp.Domain.Entities.mem;

namespace MemApp.Infrastructure.Configurations.mem;

public class VenueAddOnsItemDetailConfiguration : IEntityTypeConfiguration<VenueAddOnsItemDetail>
{
    public void Configure(EntityTypeBuilder<VenueAddOnsItemDetail> builder)
    {
        builder.ToTable("mem_VenueAddOnsItemDetail");
        builder.HasKey(e => new { e.Id });
        builder.Property(e=>e.Id).ValueGeneratedOnAdd();

        builder.HasOne(x => x.VenueBookingDetail)
           .WithMany(y => y.VenueAddOnsItemDetails)
           .HasForeignKey(z => z.BookingDetailId)
           .OnDelete(DeleteBehavior.NoAction);

    }
}
