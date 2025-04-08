using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Reports.RefundReport.VenueBookingRefund.Model
{
    public class VenueBookingRefundDetailReportVM
    {
        public string MembershipNo { get; set; }
        public string MemberName { get; set; }
        public string VenueTitle { get; set; }
        public string BookedNo { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime CancellationDate { get; set; }
        public decimal BookingPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalServiceCharge { get; set; }
        public decimal TotalVatAmount { get; set; }
        public decimal SubTotal { get; set; }
        public decimal RefundAmount { get; set; }
        public string CancellationNote { get; set; }

        public string CancelledBy { get; set; }
    }
}
