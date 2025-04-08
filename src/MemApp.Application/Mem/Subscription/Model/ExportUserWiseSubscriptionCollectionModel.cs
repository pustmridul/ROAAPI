using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Subscription.Model
{
    public class ExportUserWiseSubscriptionCollectionModel
    {
        public string UserName { get; set; }
        public string MemberShipNo { get; set; }
        public string MemberName { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime ActualPaymentDate { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal LateAmount { get; set; }
        public decimal UserCollection { get; set; }
        public string SubscriptionInfo { get; set; }
        public bool IsAdvancePayment { get; set; }
    }
}
