using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Reports.TicketReport.Model
{
    public class EventTicketReportVM
    {
        public int EventId { get; set; }
        public string EventTitle { get; set; }
        public DateTime EventDate { get; set; }
        public string TicketCriteria { get; set; }
        public int TicketCount { get; set; }
        public int MemServiceTypeId { get; set; }
        public decimal TicketPrice { get; set; }
        public decimal TotalSaleAmount { get; set; }
        public DateTime InvoiceDate { get;set; }





    }
}
