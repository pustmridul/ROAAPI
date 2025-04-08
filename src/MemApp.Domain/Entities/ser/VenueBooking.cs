using MemApp.Domain.Core.Models;
using MemApp.Domain.Entities.mem;

namespace MemApp.Domain.Entities.ser
{
    public class VenueBooking : BaseEntity
    {
        public string BookedNo { get; set; }= string.Empty;
        public DateTime BookedDate { get; set;}    
        public string BookingStatus { get; set; } = string.Empty;
        public RegisterMember Member { get; set; }
        public int MemberId { get; set; }
       
        public string? MemberShipNo { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalAmount { get; set; }

        public decimal VatAmount { get; set; }
        public decimal ServiceAmount { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal DiscountAmount { get; set; }

        public DateTime PaymentDate { get; set; }
        public bool IsActive { get; set; }
        public string? BookingCriteria { get; set; }
        public int BookingCriteriaId { get; set; }
        public decimal? BookingPrice { get; set; }
        public string? OrderFrom { get; set; }

        public string? RefName { get; set; }
        public string? RefRelation { get; set; }
        public string? Note { get; set; }
        public string? BookingPurpose { get; set; }
        public string? RefPhoneNo { get; set; }
        public bool IsTramsAndCondition { get; set; }
        public bool IsCancelled { get; set; }
        public decimal? AdditionalFee { get; set; }
        public string? AdditionalFeeNote { get; set; }

        public ICollection<VenueBookingDetail> VenueBookingDetails { get; set; }



    }
}
