using MemApp.Domain.Core.Models;
using MemApp.Domain.Entities.mem;
using System.ComponentModel.DataAnnotations;

namespace MemApp.Domain.Entities.Payment
{
    public class SubscriptionPaymentTemp:BaseEntity
    {
        public int RegisterMemberId { get; set; }
        public string PaymentNo { get; set; } = string.Empty;
        public string MemberShipNo { get; set; }=string.Empty;
        public int SubscriptionFeesId { get; set; }
        public string SubscriptionName { get; set; }= string.Empty;
        public string? SubscriptionYear { get; set; }
        public Decimal PaymentAmount { get; set; }
        public Decimal LateAmount { get; set; }
        public Decimal LateFeePer { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime? ActualPaymentDate { get; set; }
        public bool IsPaid { get; set; }
        public string? SubsStatus { get; set; }
        public Decimal AbroadFeePer { get; set; }
        public Decimal AbroadFeeAmount { get; set; }
        public RegisterMember? RegisterMember {  get; set; } 
    }

    public class SubscriptionDueTemp 
    {
        [Key]
        public int Id { get; set; }
        public int MemberId { get; set; }
        public string MemberShipNo { get; set; } = string.Empty;
        public Decimal PaymentAmount { get; set; }
        public decimal LateFeePer { get; set; }
        public Decimal LateAmount { get; set; }
        public DateTime ActualPaymentDate { get; set; }
        public DateTime SyncDate { get; set; }
        public DateTime GenerateDate { get; set; }
        public int SubscriptionFeesId { get; set; }
        public bool IsPaid { get; set; }
        public Decimal AbroadFeePer { get; set; }
        public Decimal AbroadFeeAmount { get; set; }
        public string SubscriptionName { get; set; } = string.Empty;
        public string? SubscriptionYear { get; set; }

        public bool IsQBSync { get; set; }
        public string? QBCusName { get; set; }
        public DateTime? QBSyncDate { get; set; }

    }
}
