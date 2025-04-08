using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Reports.TicketReport.Model
{
    public class EventTicketReportCriteria
    {
        public int? EventId { get; set; }
        public string? EventTicketIds { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? TicketCriteria { get; set; }

    }
}
