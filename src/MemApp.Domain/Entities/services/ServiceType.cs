using MemApp.Domain.Core.Models;
using MemApp.Domain.Entities.mem;
using MemApp.Domain.Entities.Sale;
using MemApp.Domain.Entities.ser;

namespace MemApp.Domain.Entities.services
{
    public class ServiceType : BaseEntity
    {
        public string Title { get; set; }
        public string? DisplayName { get; set; }
        public bool IsActive { get; set; } = true;
        public  ICollection<MemService> MemServices { get; set; }
        public ICollection<SaleMaster> SaleMasters { get; set; }
        
    }
}
