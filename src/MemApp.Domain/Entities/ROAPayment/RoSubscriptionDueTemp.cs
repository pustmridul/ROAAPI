using Res.Domain.Entities.ROASubscription;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Res.Domain.Entities.ROAPayment
{
    public class RoSubscriptionDueTemp
    {
        [Key]
        public int Id { get; set; }
        public int? MemberId { get; set; }

        [ForeignKey("MemberId")]
        public MemberRegistrationInfo? Member { get; set; }
        public string MemberShipNo { get; set; } = string.Empty;
        public Decimal PaymentAmount { get; set; }
        public decimal LateFeePer { get; set; }
        public Decimal LateAmount { get; set; }
        public DateTime ActualPaymentDate { get; set; }
        public DateTime SyncDate { get; set; }
        public DateTime GenerateDate { get; set; }
        public int? SubscriptionFeeId { get; set; }

        [ForeignKey("SubscriptionFeeId")]
        public ROASubscriptionFee? SubscriptionFee { get; set; }
        public bool IsPaid { get; set; }
      //  public Decimal AbroadFeePer { get; set; }
      //  public Decimal AbroadFeeAmount { get; set; }
        public string SubscriptionName { get; set; } = string.Empty;
        public int? SubscriptionYear { get; set; }
        public DateTime? SubscriptionMonth { get; set; }
        public string? GeneratedBy { get; set; }

        public bool IsQBSync { get; set; }
        public string? QBCusName { get; set; }
        public DateTime? QBSyncDate { get; set; }

        public int CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
