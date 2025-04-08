using MemApp.Domain.Core.Models;

namespace MemApp.Domain.Entities.subscription
{
    public class SubscriptionMode:BaseEntity
    {
        public string Name { get; set; }
        public double Value { get; set; }
        public int MonthLength { get; set; }
        public ICollection<SubscriptionFees> SubscriptionFees { get; set; }
    }
}
