using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Reports.RefundReport.ServiceRefund.Model
{
    public class ServiceRefundDetailReportVM
    {
        public string MembershipNo { get; set; }
        public string MemberName { get; set; }
        public string ServiceName { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime RevDate { get; set; }
        public DateTime CancellationDate { get; set; }
        public decimal UnitPrice { get; set; }
        public string TicketCriteria { get; set; }
        public int Quantity { get; set; }
        public decimal TotalServiceCharge { get; set; }
        public decimal TotalVatAmount { get; set; }
        public decimal SubTotal { get; set; }
        public decimal RefundAmount { get; set; }
        public string CancellationNote { get; set; }

        public string CancelledBy { get; set; }
    }
}
