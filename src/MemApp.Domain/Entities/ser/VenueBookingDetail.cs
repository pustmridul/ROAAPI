using MemApp.Domain.Core.Models;
using MemApp.Domain.Entities.mem;

namespace MemApp.Domain.Entities.ser
{
    public class VenueBookingDetail : BaseEntity
    {
  
        public int BookingId { get; set; }
        public int VenueId { get; set; }
        public string? VenueTitle { get; set; }
        public DateTime BookingDate { get; set; }
        public Availability Availabilities { get; set; }
        public int? AvailabilityId { get; set; }
        public VenueBooking VenueBooking { get; set; }
        public bool IsActive { get; set; }
        public bool IsCancelled { get; set; }
        public string? SaleStatus { get; set; }
        public string? CancellationNote { get; set; }
        public decimal? RefundAmount { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal VatPercent { get; set; }
        public decimal? ServiceChargeAmount { get; set; }
        public virtual ICollection<VenueAddOnsItemDetail>? VenueAddOnsItemDetails { get; set; } = new List<VenueAddOnsItemDetail>();



    }
}
