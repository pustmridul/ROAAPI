using MemApp.Domain.Core.Models;

namespace MemApp.Domain.Entities.ser
{
    public class SaleEventTicket : BaseEntity
    {
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set;}
        public string SaleStatus { get; set; }
        public int MemberId { get; set; }
        public string? MemberShipNo { get; set; }
        public decimal Amount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal VatPercent { get; set; }
        public decimal ServiceAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaymentAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public bool IsActive { get; set; }
    
        public string? OrderFrom { get; set; }
    

        public ICollection<SaleEventTicketDetail> SaleEventTicketDetails { get; set; }

    }
}
