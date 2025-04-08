using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Reports.ServiceReport.Model
{
    public class ServiceSaleReportVM
    {
        public string? InvoiceNo { get; set; }
        public string? MemberName { get; set; }
        public string? MembershipNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime BookedDate { get; set; }
        public string? ServiceName { get; set; }
        public string? TicketCriteria { get; set; }
        public string? TicketQuantity { get; set; }
        public decimal? TicketPrice { get; set; }
        public decimal? VAT { get; set; }
        public decimal? ServiceFee { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? PaidAmount { get; set; }
        public string? BookedBy { get; set; }
        public int ServiceTicketId { get; set; }
        public int MemServiceId { get; set; }


    }
}
