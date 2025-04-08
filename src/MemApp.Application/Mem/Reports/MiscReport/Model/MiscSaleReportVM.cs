using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Reports.MiscReport.Model
{
    public class MiscSaleReportVM
    {
        public string InvoiceNo { get; set; } = string.Empty;
        public string MemberName { get; set; }=string.Empty;
        public string MembershipNo { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public string ItemText { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }

    }
}
