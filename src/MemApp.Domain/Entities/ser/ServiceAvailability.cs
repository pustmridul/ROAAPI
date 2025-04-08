using MemApp.Domain.Core.Models;
using System.Text.Json.Serialization;

namespace MemApp.Domain.Entities.ser
{
    public class ServiceAvailability : BaseEntity
    {
        public int? ServiceTicketId { get; set; }
        public DateTime AvailabiltyDate { get; set; }
        public string Morning { get; set; }
        public string Afternoon { get; set; }
        public string Evening { get; set; }
        public string? WholeDay { get; set; }
        
    }
}
