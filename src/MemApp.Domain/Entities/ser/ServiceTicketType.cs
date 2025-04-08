using MemApp.Domain.Core.Models;
using MemApp.Domain.Entities.Sale;
using MemApp.Domain.Entities.services;
using System.Text.Json.Serialization;

namespace MemApp.Domain.Entities.ser
{
    public class ServiceTicketType:BaseEntity
    {
        public string Title { get; set; }
        public bool IsActive { get; set; }     
        public string? ServiceType { get; set; }
        [JsonIgnore]
        public ICollection<ServiceTicketDetail>? ServiceTicketDetails { get; set; }
    }
}
