using MemApp.Domain.Core.Models;
using MemApp.Domain.Entities.ser;
using MemApp.Domain.Entities.services;

namespace MemApp.Domain.Entities.Sale
{
    public class SaleMaster:BaseEntity
    {
        public decimal ExpenseAmount { get; set; }
        public decimal ServiceChargePercent { get; set; }
        public decimal ServiceChargeAmount { get; set; }
        public decimal VatChargePercent { get; set; }
        public decimal VatChargeAmount { get; set; }
        public ServiceType ServiceTypes { get; set; }
        public int ServiceTypeId  { get; set; }
        public MemService MemServices { get; set; }
        public int ServiceTicketId { get; set; }
        public ServiceTicket ServiceTicket { get; set; }
        public int MemServiceId { get; set; }
        public int MemberId { get; set; }
        public string MemberShipNo { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string InvoiceStatus { get; set; }
        public string SaleType { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ReservationDate { get; set; }
        public string OrderFrom { get; set; }


        public ICollection<SaleTicketDetail> SaleTicketDetails { get; set; }
        public ICollection<SaleLayoutDetail> SaleLayoutDetails { get; set; }


    }
}
