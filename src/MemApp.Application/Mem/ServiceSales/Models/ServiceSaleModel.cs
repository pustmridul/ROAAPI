using MemApp.Application.Extensions;
using MemApp.Application.Mem.TopUps.Models;

namespace MemApp.Application.Mem.ServiceSales.Models
{
    public class ServiceSaleModel
    {
    }
    public class ServiceSaleReq
    {
        public int Id { get; set; }
        public int? MemberId { get; set; }
        public string? InvoiceNo { get; set; }
        public string? MembershipNo { get; set; }
        public string? OrderFrom { get; set; }
        public string InvoiceDate { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal ServiceChargeAmount { get; set; }
        public string? MemberName { get; set; }
        public string? MemberEmail { get; set; }
        public string? ImgFileUrl { get; set; }
        public string? Note { get; set; }
        public List<ServiceSaleDetailReq> ServiceSaleDetailReqs { get; set; } = new List<ServiceSaleDetailReq>();

        public TopUpReq? topup { get; set; }
        public string? PhoneNo { get; set; }
        public string? SmsText { get; set; }
    }
    public class ServiceSaleDetailReq
    {
        public int Id { get; set; }
        public int SaleYear { get; set; }
        public int SaleMonth { get; set; }
        public int SaleDay { get; set; }
        public int ServiceSaleId { get; set; }
        public int ServiceTicketId { get; set; }
        public string? TicketText { get; set; }
        public int ServiceCriteriaId { get; set; }
        public string? ServiceCriteriaText { get; set; }

        public string? UnitName { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? Quantity { get; set; }
        public int SlotQty { get; set; }

        public bool IsActive { get; set; }
        public string? TicketCodeNo { get; set; }
        public string? Note { get; set; }
        public decimal? VatChargeAmount { get; set; }
        public decimal VatChargePercent { get; set; }
        public decimal ServiceChargeAmount { get; set; }
        public decimal ServiceChargePercent { get; set; }
        public decimal SubTotal { get; set; }
        public DateTime? RevDate { get; set; }
        public string? DayText { get; set; } = string.Empty;
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public bool IsWholeDay { get; set; }
        public decimal ServicePrice { get; set; }
        public decimal ServiceQty { get; set; }
        public int? SeviceTicketAvailablityId { get; set; }
        public int? MemServiceId { get; set; }
    }
    public class SaleTicketAvailabilityReq
    {
        public int Id { get; set; }
        public int ServiceTicketId { get; set; }
        public string Day { get; set; } = string.Empty;
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public bool IsWholeDay { get; set; }

    }
    public class ServiceSaleVm : Result
    {
        public ServiceSaleReq Data { get; set; } = new ServiceSaleReq();
    }

    public class ServiceSaleListVm : Result
    {
        public long DataCount { get; set; }
        public List<ServiceSaleReq> DataList { get; set; } = new List<ServiceSaleReq>();
    }

    public class ServiceSaleSearch
    {
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string? SearchText { get; set; }

        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public int? TicketCriteriaId { get; set; }

    }
    public class ServiceSaleTicketReq : Result
    {
        public int ServiceSaleId { get; set; }
        public string TicketTitle { get; set; } = string.Empty;
        public List<ServiceSaleTicketDetailReq> TicketDetails { get; set; } = new List<ServiceSaleTicketDetailReq>();
        public string MemberShipNo { get; set; } = string.Empty;
        public string MemberName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string InvoiceNo { get; set; } = string.Empty;
        public string InvoiceDate { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string CreatedByName { get; set; }

    }
    public class ServiceSaleTicketDetailReq
    {
        public decimal TicketPrice { get; set; }
        public string TicketCodeNo { get; set; } = string.Empty;
        public int TicketQuantity { get; set; }
        public string TicketDate { get; set; } = string.Empty;
        public string TicketType { get; set; } = string.Empty;
        public string ServiceText { get; set; } = string.Empty;
        public decimal VatAmount { get; set; }
        public decimal ServiceCharge { get; set; }

    }
}
