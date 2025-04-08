using MemApp.Domain.Core.Models;
using MemApp.Domain.Entities.mem;
using MemApp.Domain.Entities.Sale;

namespace MemApp.Domain.Entities.ser
{
    public class ServiceTicket : BaseEntity
    {
        public string? Title { get; set; }
        public string? DisplayTitle { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ValidTo { get; set; }
        public DateTime? EventDate { get; set; }
        public string? Description { get; set; }
        public bool HasAvailability { get; set; }
        public bool HasTicket { get; set; }
        public bool HasAreaLayout { get; set; }
        public string? PromoCode { get; set; }
        public string? Location { get; set; }
        public decimal ServiceChargePercent { get; set; }
        public decimal ServiceChargeAmount { get; set; }
        public decimal VatChargePercent { get; set; }
        public decimal VatChargeAmount { get; set; }
        public int? MemServiceId { get; set; }
        public MemService MemServices { get; set; }
        public int? MemServiceTypeId { get; set; }
        public int? AvailabilityId { get; set; }

        public Availability? Availability { get; set; }
        public string? ImgFileUrl { get; set; }
        public bool IsValid { get; set; }
        public bool IsActive { get; set; }
        public bool HasToken { get; set; }
        public int TicketLimit { get; set; }
        public bool Status { get; set; }
        public virtual List<ServiceTicketDetail> ServiceTicketDetails { get; set; } = new List<ServiceTicketDetail>();
        public virtual List<SerTicketAreaLayout> SerTicketAreaLayouts { get; set; } = new List<SerTicketAreaLayout>();
        public virtual List<EventToken> EventTokens { get; set; } = new List<EventToken>();
        public virtual List<ServiceTicketAvailability> ServiceTicketAvailabilities { get; set; } = new List<ServiceTicketAvailability>();
        public virtual ICollection<ServiceSaleDetail> ServiceSaleDetails { get; set; } 





        public ICollection<SaleMaster> SaleMasters { get; set; }
    }
}
