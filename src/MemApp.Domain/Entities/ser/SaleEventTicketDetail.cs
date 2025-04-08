using MemApp.Domain.Core.Models;

namespace MemApp.Domain.Entities.ser
{
    public class SaleEventTicketDetail : BaseEntity
    {
        public int SaleEventId { get; set; }
        public int EventId { get; set; }
        public string? EventTitle { get; set; }
        public string? EventTokens { get; set; }
        public int? AreaLayoutId { get; set; }
        public string? AreaLayoutTitle { get; set; }
        public int? TableId { get; set; }
        public string? TableTitle { get; set; }
        public int? NoofChair { get; set; }
        public SaleEventTicket SaleEventTicket { get; set; }
        public bool IsActive { get; set; }
        public string? TicketCodeNo { get; set; }
        public string? TicketCriteria { get; set; }
        public int TicketCriteriaId { get; set; }
        public decimal? TicketPrice { get; set; }
        public string? TicketText { get;set; }
        public ServiceTicket Event { get; set; }
        public string? SaleStatus { get; set; }
        public string? CancellationNote { get; set; }
        public decimal? RefundAmount { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal VatPercent { get; set; }
        public decimal? ServiceChargeAmount { get; set; }
    }
}
