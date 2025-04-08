using MemApp.Domain.Core.Models;
using MemApp.Domain.Entities.ser;

namespace MemApp.Domain.Entities.mem
{
    public class ServiceSaleDetail : BaseEntity
    {
        public int ServiceSaleId { get; set; }
        public int ServiceTicketId { get; set; }
        public int ServiceCriteriaId { get; set; }
        public string? UnitName { get; set; }
        public int RevYear { get; set; }
        public int RevMonth { get; set; }
        public int RevDay { get; set; }
        public DateTime RevDate { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? Quantity { get; set; }
        public bool IsActive { get; set; }
        public ServiceSale ServiceSale { get; set; }
        public string? TicketCodeNo { get; set; }
        public string? SaleStatus { get; set; }
        public string? CancellationNote { get; set; }
        public ServiceTicket ServiceTicket { get; set; }
        public decimal RefundAmount { get; set; }
        public string? DayText { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public bool IsWholeDay { get; set; }
        public string? ServiceCriteriaText { get; set; }
        public string? TicketText { get; set; }
        public int? SeviceTicketAvailablityId { get; set; }
        public decimal ServiceChargeAmount { get; set; }
        public decimal ServiceChargePercent { get; set; }
        public decimal VatChargePercent { get; set; }
        public decimal VatChargeAmount { get; set; }
        public int? MemServiceId { get; set; }

    }
}
