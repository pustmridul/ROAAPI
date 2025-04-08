using MemApp.Application.Extensions;
using MemApp.Application.Mem.MemServices.Models;
using MemApp.Application.Mem.TopUps.Models;

namespace MemApp.Application.Mem.Events.Models
{
    public class SaleEventTicketModel
    {
    }
    public class SaleEventTicketReq
    {
        public int Id { get; set; }
        public string? InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string? SaleStatus { get; set; }
        public int MemberId { get; set; }
        public string? MemberEmail { get; set; }

        public string? MemberShipNo { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal ServiceAmount { get; set; }
        public decimal PaymentAmount { get; set; }
        public DateTime? PaymentDate { get; set; }

        public string? OrderFrom { get; set; }
        public decimal RefundAmount { get; set; }

        public List<SaleEventTicketDetailReq> SaleEventTicketDetailReqs { get; set; } = new List<SaleEventTicketDetailReq>();
        public string? SmsText { get; set; }
        public string? PhoneNo { get; set; }
        public TopUpReq? TopUpReq { get; set; }
    }
    public class SaleEventTicketDetailReq
    {
        public int Id { get; set; }
        public int SaleEventId { get; set; }
        public int EventId { get; set; }
        public string? EventTitle { get; set; }
        public string? EventTokens { get; set; }

        public int? AreaLayoutId { get; set; }
        public string? AreaLayoutTitle { get; set; }
        public int? TableId { get; set; }
        public string? TableTitle { get; set; }
        public int? NoofChair { get; set; }
        public string? TicketCriteria { get; set; }
        public int TicketCriteriaId { get; set; }
        public decimal? TicketPrice { get; set; }
        public string? TicketText { get; set; }
        public string? Note { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal? VatPercentage { get; set; }
        public decimal? ServiceChargeAmount { get; set; }
        public decimal? ServiceChargePercentage { get; set; }
        public ServiceTicketReq Event { get; set; }
    }

    public class SaleEventTicketVm : Result
    {
        public SaleEventTicketReq Data { get; set; } = new SaleEventTicketReq();
    }

    public class SaleEventTicketListVm : Result
    {
        public List<SaleEventTicketReq> DataList { get; set; } = new List<SaleEventTicketReq>();
        public long DataCount { get; set; }
    }

    public class SaleEventTicketSearchReq
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string? SearchText { get; set; }
        public int? TicketCriteriaId { get; set; }

    }

    public class EventTictetReq
    {
        public string? Location { get; set; }
        public string EventTitle { get; set; }
        public List<string> EventTokens { get; set; } = new List<string>();
        public string TicketCodeNo { get; set; }
        public string MemberShipNo { get; set; }
        public string MemberName { get; set; }
        public decimal TicketPrice { get; set; }
        public string? TicketCriteria { get; set; }
        public string InvoiceNo { get; set; }
        public string EventDate { get; set; }
        public string InvoiceDate { get; set; }
        public string ChairNo { get; set; }
        public string TableTitle { get; set; }
        public string AreaLayoutTitle { get; set; }
        public string? TicketText { get; set; }
    }

    public class EventTicketListVm : Result
    {
        public List<EventTictetReq> DataList { get; set; } = new List<EventTictetReq>();
    }

    public class EventTicketBuyInfo : Result
    {
        public int TotalSaleCount { get; set; }
        public List<MemberTicketCriteriaBuyInfo> MemberTicketCriteriaBuyInfos { get; set; } = new List<MemberTicketCriteriaBuyInfo>();
    }

    public class MemberTicketCriteriaBuyInfo
    {
        public int TicketCriteriaId { get; set; }
        public int TicketCriteriaCount { get; set; }

    }
}
