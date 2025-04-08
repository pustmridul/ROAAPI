using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Subscription.Model
{
    public class SubscriptionPaymentReportReq
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string MembershipNo { get; set; }
        public string Year { get; set; }
        public string Quarter { get; set; }
    }
}
