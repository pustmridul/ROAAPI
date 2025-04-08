using MemApp.Domain.Core.Models;
using MemApp.Domain.Entities.mem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Res.Domain.Entities.ROAPayment
{
    public class RoaMembershipFeePayment:BaseEntity
    {
        public int MemberId { get; set; }
        public MemberRegistrationInfo Member { get; set; }
      //  public int MemberShipFeeId { get; set; }
       // public MemberShipFee MemberShipFee { get; set; }

        public string PaymentNo { get; set; } = string.Empty;
        public string? MemberFeesTitle { get; set; }
        public decimal Amount { get; set; }
        public bool IsActive { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
