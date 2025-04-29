using MemApp.Application.Mem.TopUps.Models;
using ResApp.Application.ROA.SubscriptionPayment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResApp.Application.ROA.MembershipFee.Models
{
    public class MembershipFeePayReq
    {
        public int MemberId { get; set; }
        public decimal Amount { get; set; }
        public TopUpReq? topup { get; set; }
    }

    public class MembershipFeePymentRes
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public string? MemberName { get; set; }
        public string? MembershipNo { get; set; }
        public string PaymentNo { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
