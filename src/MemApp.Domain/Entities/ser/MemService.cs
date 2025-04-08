using MemApp.Domain.Core.Models;
using MemApp.Domain.Entities.mem;
using MemApp.Domain.Entities.Sale;
using MemApp.Domain.Entities.services;

namespace MemApp.Domain.Entities.ser
{
    public class MemService : BaseEntity
    {
        public int ServiceTypeId { get; set; }
        public string Title { get; set; }
        public string? DisplayTitle { get; set; }
        public bool IsActive { get; set; }
        public ServiceType ServiceTypes { get; set; }
        public ICollection<ServiceTicket> ServiceTickets { get; set; }
        public ICollection<SaleMaster> SaleMasters { get; set; }

        

    }
}
