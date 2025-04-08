using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Reports.MemberReport.Model
{
    public class MemberLedgerDetailReportVM
    {
        public int RegisterMemberId { get; set; }
        public string? MemberName { get; set; }
        public string? MembershipNo { get; set; }
        public string? Phone { get; set; }
        public string? CardNo { get; set; }
        public DateTime? Dates { get; set; }
        public string Description { get; set; }
        public string LedgerType { get; set; }

        public string PayType { get; set; }
        public string BankCreditCardName { get; set; }
        public decimal Amount { get; set; }
        public string? Email { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal Balance { get; set; }
    }
}
