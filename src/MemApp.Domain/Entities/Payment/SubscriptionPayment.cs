using MemApp.Domain.Core.Models;
using MemApp.Domain.Entities.mem;

namespace MemApp.Domain.Entities.Payment
{
    public class SubscriptionPayment:BaseEntity
    {
        public int RegisterMemberId { get; set; }
        public string MemberShipNo { get; set; } = string.Empty;
        public RegisterMember RegisterMembers { get; set; }
        public Decimal MemberPayment { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime? ActualPaymentDate { get; set; }
        public bool IsPaid { get; set; }

        public string? SubsStatus { get; set; }
        public ICollection<SubscriptionPaymentDetail> SubscriptionPaymentDetails { get; set; }
    }
}
