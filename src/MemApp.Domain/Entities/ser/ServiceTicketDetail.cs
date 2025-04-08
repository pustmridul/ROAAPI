using MemApp.Domain.Core.Models;
using MemApp.Domain.Entities.Sale;
using System.Text.Json.Serialization;

namespace MemApp.Domain.Entities.ser
{
    public class ServiceTicketDetail : BaseEntity
    {
        public int ServiceTicketId { get; set; }
        public int? ServiceTicketTypeId { get; set; }
        public ServiceTicketType? ServiceTicketType { get; set; }
        public string TicketType { get; set; }= string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int MaxQuantity { get; set; }
        public int MinQuantity { get; set; }
        [JsonIgnore]
        public ServiceTicket ServiceTicket { get; set; }
        //public ICollection<ServiceTicketAvailability> ServiceTicketAvailabilities { get; set; }
        public ICollection<SaleTicketDetail> SaleTicketDetails { get; set; }
    }
}
