using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Reports.VenueReport.Model
{
    public class VenueBookingReportVM
    {
        public string MembershipNo { get; set; }
        public string MemberName { get; set; }
        public DateTime BookedDate { get; set; }
        public DateTime BookingDate { get; set; }

        public string BookedNo { get; set; }
        public string VenueName { get; set; }
        public string AddOnName { get; set; }
        public string VenueShift { get; set; }
        public string BookingPurpose { get; set; }
        public string ReferenceName { get; set; }
        public decimal VenueFee { get; set; }
        public decimal AddOnFee { get; set; }
        public decimal ServiceCharge { get; set; }
        public decimal VAT { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal DiscountAmount { get; set; }   
        public decimal DueAmount { get; set; }
        public string BookedBy { get; set; }
        public int VenueId { get; set; }
        public DateTime PaymentDate { get; set; }
    
    }
}
