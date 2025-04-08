using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Reports.TicketReport.Model
{
    public class EventTicketSaleReportVM
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public string MemberShipNo { get; set; }
        public string TicketCriteria { get; set; }
        public int TicketCount { get; set; }
    }
}
