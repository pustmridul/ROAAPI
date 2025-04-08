using MemApp.Domain.Core.Models;

namespace MemApp.Domain.Entities.com
{
    public class PaymentMethod 
    {
        public int PaymentTypeID { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<TopUpDetail> TopUpDetails { get; set; }
    }
}
