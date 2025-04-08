using MemApp.Domain.Core.Models;
using MemApp.Domain.Entities.Payment;

namespace MemApp.Domain.Entities.subscription
{
    public class SubscriptionFees:BaseEntity
    {
        public string SubscribedYear { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int SubscriptionModId { get; set; }
        public Decimal SubscriptionFee { get; set; }
        public Decimal? LateFee { get; set; }
        public Decimal? AbroadFee { get; set; }
        public bool IsActive { get; set; }
        public int MonthLength { get; set; }
        public SubscriptionMode SubscriptionMode { get; set; }
        public ICollection<SubscriptionPaymentDetail> SubscriptionPaymentDetails { get; set; }
    }
}
