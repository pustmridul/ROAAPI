using MemApp.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MemApp.Domain.Entities.ser
{
    public class EventToken : BaseEntity
    {
        public string? TokenTitle { get; set; }
        public string TokenCode { get; set; }
        public bool IsActive { get; set; }
        public int ServiceTicketId { get; set; }
        [JsonIgnore]
        public ServiceTicket ServiceTicket { get; set; }
    }
}
