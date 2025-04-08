using MemApp.Domain.Core.Models;
using MemApp.Domain.Entities.mem;


namespace MemApp.Domain.Entities.com
{
    public class Feedback : BaseEntity
    {
        public int MemberId { get; set; }
        public int? FeedbackCategoryId { get; set; }
        public string? MemberShipNo { get; set; }
        public string? Message { get; set; }
        public DateTime FeedbackDate { get; set; } = DateTime.Now;

        public ICollection<Reply>? Replies { get; set; }
        public RegisterMember Member { get; set; }
        public int Status { get; set; }
        public FeedbackCategory FeedbackCategory { get; set; }
    }
}
