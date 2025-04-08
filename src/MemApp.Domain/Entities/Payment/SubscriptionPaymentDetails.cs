using MemApp.Domain.Core.Models;
using MemApp.Domain.Entities.mem;
using MemApp.Domain.Entities.subscription;

namespace MemApp.Domain.Entities.Payment
{
    public class SubscriptionPaymentDetail : BaseEntity
    {
        public Decimal CustomerDue { get; set; }
        public int SubscriptionFeesId { get; set; }
        public DateTime PaymentFeesDate { get; set; }
        public Decimal PaymentFees { get; set; }
        public decimal LateFees { get; set; }
        public bool IsPaid { get; set; }

        public SubscriptionFees SubscriptionFees { get; set; }
        public int SubscriptionPaymentId { get; set; }
        public SubscriptionPayment SubscriptionPayments { get; set; }
        public int? RegisterMemberId { get; set; }
        public RegisterMember RegisterMembers { get; set; }
    }
}
