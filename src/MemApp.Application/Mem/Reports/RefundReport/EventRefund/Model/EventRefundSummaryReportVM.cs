using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Reports.RefundReport.EventRefund.Model
{
    public class EventRefundSummaryReportVM
    {
        public string MembershipNo { get; set; }
        public string MemberName { get; set; }
        public string SubTotal { get; set; }
        public string RefundAmount { get; set; }
        public DateTime CancellationDate { get; set; }
    }
}
