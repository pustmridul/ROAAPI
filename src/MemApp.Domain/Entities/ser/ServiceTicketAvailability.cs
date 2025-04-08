using MemApp.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Domain.Entities.ser
{
    public class ServiceTicketAvailability:BaseEntity
    {
        public int ServiceTicketId { get; set; }
        public ServiceTicket? ServiceTickets { get; set; }
        public int Qty { get; set; }
        public string DayText { get; set; } = string.Empty;
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public bool IsWholeDay { get; set; }
        public int SlotId { get; set; }

    }
}
