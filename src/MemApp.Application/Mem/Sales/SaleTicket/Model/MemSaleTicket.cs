using MemApp.Application.Extensions;
using MemApp.Application.Models;

namespace MemApp.Application.Mem.Sales.SaleTicket.Model
{
    public class MemSaleTicket
    {
    }
    public class MemSaleTicketReq
    {
        public int Id { get; set; }
        public int ServiceTicketId { get; set; }
        public int ServiceTypeId { get; set; }
        public int MemServiceId { get; set; }
        public string MembershipNo { get; set; } = string.Empty;
        public int MemberId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string? InvoiceNo { get; set; }
        public string InvoiceStatus { get; set; } = string.Empty;
        public string SaleType { get; set; } = string.Empty;
        public List<SaleTicketDetailReq>? SaleTicketDetailReqs { get; set; } = new List<SaleTicketDetailReq>();
        public List<SaleLayoutDetailReq>? SaleLayoutDetailReqs { get; set; } = new List<SaleLayoutDetailReq>();
        public decimal? ExpenseAmount { get; set; }
        public decimal? ServiceChargePercent { get; set; }
        public decimal? ServiceChargeAmount { get; set; }
        public decimal? VatChargePercent { get; set; }
        public decimal? VatChargeAmount { get; set; }
        public DateTime? ReservationDate { get; set; }
        public string OrderFrom { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public decimal Amount { get; set; }

    }
    public class MemSaleTicketRes
    {
        public int Id { get; set; }
        public decimal ExpenseAmount { get; set; }
        public decimal ServiceChargePercent { get; set; }
        public decimal ServiceChargeAmount { get; set; }
        public decimal VatChargePercent { get; set; }
        public decimal VatChargeAmount { get; set; }
        public int ServiceTypeId { get; set; }
        public int ServiceTicketId { get; set; }
        public int MemServiceId { get; set; }
        public int ServiceTicketTypeId { get; set; }
        public string InvoiceNo { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public List<SaleTicketDetailsRes> SaleTicketDetailsRess { get; set; } = new List<SaleTicketDetailsRes>();
    }

    public class SaleTicketDetailReq
    {
        public int Id { get; set; }
        public int ServiceTickerDetailId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? UnitName { get; set; }
       


    }

   


    public class SaleLayoutDetailReq
    {
        public int Id { get; set; }
        public int AreaLayoutId { get; set; }
        public string? AreaLayoutTitle { get; set; }
        public int TableId { get; set; }
        public string? TableName { get; set; }
        public int NoofChair { get; set; }
    }
    public class SaleTicketDetailsRes
    {
        public int Id { get; set; }
        public int ServiceTickerDetailId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? UnitName { get; set;}
    }
    public class MemSaleTicketVm : Result
    {
        public MemSaleTicketReq Data { get; set; } = new MemSaleTicketReq();
    }
    public class MemSaleTicketListVm : Result
    {
        public long DataCount { get; set; }
        public List<MemSaleTicketRes> DataList { get; set; } = new List<MemSaleTicketRes>();
    }
    public class SaleTicketSearchReq
    {
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string? SearchText { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? ServiceTypeId { get; set; }
        public int? ServiceId { get; set; }
        public string? OrderFrom { get; set;}
    }

}
