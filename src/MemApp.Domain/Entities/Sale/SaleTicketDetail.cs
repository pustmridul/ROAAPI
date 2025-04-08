using MemApp.Domain.Core.Models;
using MemApp.Domain.Entities.ser;

namespace MemApp.Domain.Entities.Sale
{
    public class SaleTicketDetail : BaseEntity
    {
        public SaleMaster SaleMaster { get; set; }
        public int SaleMasterId { get; set; }
        public ServiceTicketDetail ServiceTicketDetail { get; set; }
        public int ServiceTicketDetailId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? UnitName { get; set; }
       
        // public ICollection<SaleTicketAvailability> SaleTicketAvailabilities { get; set; }
    

    }
}
