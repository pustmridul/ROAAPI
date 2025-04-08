using MemApp.Domain.Core.Models;
using MemApp.Domain.Entities.ser;
using MemApp.Domain.Entities.services;
using System.Text.Json.Serialization;

namespace MemApp.Domain.Entities.mem
{
    public class Availability : BaseEntity
    {
        public DateTime AvailabiltyDate { get; set; }   
        public bool IsActive { get; set; }
        public bool IsLifeTime { get; set; }
        public string? Name { get; set; }
        public ICollection<AvailabilityDetail> AvailabilityDetails { get; set; }
        public ICollection<ServiceTicket> ServiceTickets { get; set; }

    }
}
