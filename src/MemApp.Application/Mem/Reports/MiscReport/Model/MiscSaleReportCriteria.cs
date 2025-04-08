using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Reports.MiscReport.Model
{
    public class MiscSaleReportCriteria
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? MembershipNo { get; set; }
    }
}
