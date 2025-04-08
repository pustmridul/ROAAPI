using MemApp.Domain.Core.Models;

namespace MemApp.Domain.Entities
{
    public class SmsLog : BaseEntity
    {
        public string PhoneNo { get; set; }
        public DateTime SmsDate { get; set; }
        public string? MemberName { get; set; }
        public string? MemberShipNo { get; set; }
        public string? Message { get; set; }
        public string Status { get; set; }
    }
}
