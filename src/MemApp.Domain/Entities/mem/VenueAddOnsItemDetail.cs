using MemApp.Domain.Core.Models;
using MemApp.Domain.Entities.ser;

namespace MemApp.Domain.Entities.mem
{
    public class VenueAddOnsItemDetail : BaseEntity
    {
        public int AddOnsItemId { get; set; }
        public int BookingId { get; set; }
        public int? BookingDetailId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public DateTime PriceDate { get; set; }
        public bool IsActive { get; set;}




        public VenueBookingDetail? VenueBookingDetail { get; set; }
    }
}
