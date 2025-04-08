using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Reports.CommonReport.Model
{
    public class AllSalesReportVm
    {
        public string? TransactionType { get; set; } = string.Empty;
        public decimal? Amount { get; set; }
        public string? MembershipNo { get; set; }
        public string? MemberName { get; set; }


    }
}
