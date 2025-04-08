using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Com.Models
{
    public class FeedbackReq
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int FeedbackCategoryId { get; set; }
        public string? MemberShipNo { get; set; }
        public string? MemberName { get; set; }
        public string? Message { get; set; }
        public int? Status { get; set; }
        public string? FeedbackCategoryName { get; set; }
        
        public DateTime FeedbackDate { get; set; } = DateTime.Now;
        public List<ReplyReq>? ReplyReqs { get; set; }
    }

    public class ReplyReq
    {
        public int Id { get; set; }
        public int FeedbackId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? Message { get; set; }
        public DateTime ReplyDate { get; set; }
    }

    public class FeedbackStatusReq
    {
        public int FeedbackId { get; set; }
        public int FeedbackStatus { get; set; }
        
    }
}
