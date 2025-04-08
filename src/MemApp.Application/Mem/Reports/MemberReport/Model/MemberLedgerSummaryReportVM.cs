using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Reports.MemberReport.Model
{
    public class MemberLedgerSummaryReportVM
    {
        public string? MembershipNo { get; set; }
        public string? Phone { get; set; }
        public string? MemberName { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
        public decimal OpeningBalance { get; set; }
    }
}
