using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Reports.ServiceReport.Model
{
    public class ServiceSaleReportCriteria
    {
        public int? ServiceId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string MembershipNo { get; set; }= string.Empty;
        public string ServiceTicketIds { get; set; } = string.Empty;
    }
}
