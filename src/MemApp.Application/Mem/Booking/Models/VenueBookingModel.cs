using MemApp.Application.Extensions;
using MemApp.Application.Mem.TopUps.Models;
using MemApp.Application.Models;
using MemApp.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Booking.Models
{
    public class VenueBookingModel
    {
    }
    public class VenueBookingReq
    {
        public int Id { get; set; }
        public string BookedNo { get; set; }
        public string BookedDate { get; set; }
        public string BookingStatus { get; set; }
        public int MemberId { get; set; }
        public string? MemberShipNo { get; set; }
        public string? MemberName { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal VatAmount { get; set; }

        public decimal ServiceAmount { get; set; }
        public decimal PaymentAmount { get; set; }
        public string PaymentDate { get; set; }
        public string? BookingCriteria { get; set; }
        public int BookingCriteriaId { get; set; }
        public decimal? BookingPrice { get; set; }
        public decimal? VatPercentage { get; set; }
        public decimal? ServicePercent { get; set; }
        public string? OrderFrom { get; set; }
        public string? RefName { get; set; }
        public string? RefRelation { get; set; }
        public string? Note { get; set; }
        public string? BookingPurpose { get; set; }
        public string? RefPhoneNo { get; set; }
        public bool IsTramsAndCondition { get; set; }
        public bool IsCancelled { get; set; }
        public string? MemberEmail { get; set; }
        public string? CreatedDate { get; set; }
        public decimal? AdditionalFee { get; set; }
        public string? AdditionalFeeNote { get; set; }


        public List<VenueBookingAddOnsItemReq>? VenueBookingAddOnsItemReqs { get; set; } = new List<VenueBookingAddOnsItemReq>();

        public List<VenueBookingDetailReq> VenueBookingDetailReqs { get; set; }= new List<VenueBookingDetailReq>();

    }

    public class VenueBookingAddOnsItemReq
    {
        public int Id { get; set; }
        public int AddOnsItemId { get; set; }
        public int BookingDetailId { get; set; }
        public int BookingId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public DateTime PriceDate { get; set; }

    }
    public class VenueBookingDetailReq
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int VenueId { get; set; }
        public string? VenueTitle { get; set; } 
        public string BookingDate { get; set; }
        public int? AvailabilityId { get; set; }
        public string? AvailabilityTitle { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public string? Note { get; set; }
        public decimal? RefundAmount { get; set; }
        public bool IsCancelled { get; set; }
        public string? CancelNote { get; set; }
        public decimal VatPercentage { get; set; }
        public decimal ServicePercent { get; set; }
        public decimal VatAmount { get; set; }
        public decimal ServiceChargeAmount { get; set; }
        public decimal TicketPrice { get; set; }
        public List<VenueBookingAddOnsItemReq>? VenueBookingAddOnsItemReqs { get; set; } = new List<VenueBookingAddOnsItemReq>();



    }

    public class BookingInfoReq
    {
        public string MemberShipNo { get; set; }
        public string MemberName { get; set; }
        public string PhoneNo { get; set; }
    }
    public class BookingInfoVm: Result
    {
        public BookingInfoReq Data { get; set; } = new BookingInfoReq();
    }
    public class VenueBookingVm : Result
    {
        public VenueBookingReq Data { get; set; }= new VenueBookingReq();
    }

    public class VenueBookingListVm : Result
    {
        public List<VenueBookingReq> DataList { get; set; } = new List<VenueBookingReq>();
        public long DataCount { get; set; }
    }

    public class VenueBookingSearchReq
    {
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string? SearchText { get; set; }
        public int? TicketCriteriaId { get; set; }
        public string? BookingStatus { get; set; }
    }
    public class VenueBookingPaymentReq
    {
        public int VenueBookingId { get; set; }
        public decimal PaymentAmount { get; set; }
        public string PaymentDate { get; set; }
        public TopUpReq? TopUpReq { get; set; }
    }
    public class TramsAndConditionVm
    {
        public string Title { get; set; }
        public List<string> Lists { get; set; } = new List<string>();
    }
    public class VenueBlockedMultipleReq
    {
        public DateTime? BlockedDate { get; set; }
        public bool IsThisMonth { get; set; }
        public bool IsThisYear { get; set; }
        public string? DayName { get; set; }
        public string? VenueTitle { get; set; }
        public List<int> VenueIds { get; set; } = new List<int>();
        public List<int> AvailibityIds { get; set; } = new List<int>();

        public string? SelectedDate { get; set; }

    }

    public class VenueBlockedSetupReq
    {
        public DateTime? BlockedDate { get; set; }
        public bool IsThisMonth { get; set; }
        public bool IsThisYear { get; set; }
        public string? DayName { get; set; }
        public string? VenueTitle { get; set; }
        public int? VenueId { get; set; }
        public string? SelectedDate { get; set; }

    }
    public class VenueBlockedListVm : Result
    {
        public List<VenueBlockedSetupReq> DataList { get; set; } = new List<VenueBlockedSetupReq>();
        public long DataCount { get; set; }
    }
}
