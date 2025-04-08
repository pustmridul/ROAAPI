using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Reports.VenueReport.Model
{
    public class VenueBookingReportCriteria
    {
        public int? VenueId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string VenueTicketIds { get; set; }
    }
}
