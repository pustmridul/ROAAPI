using MemApp.Domain.Core.Models;


namespace MemApp.Domain.Entities.com
{
    public class Reply : BaseEntity
    {
        public int FeedbackId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }= string.Empty;
        public string? Message { get; set; }
        public DateTime ReplyDate { get; set; }

        public Feedback? Feedback { get; set; }
    }
}
