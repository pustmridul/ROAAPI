
namespace MemApp.Application.Com.Models
{
    public class MessageTemplateReq
    {
        public int Id { get; set; }
        public int MessageTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Subject { get; set; }
        public string? Message { get; set; }
        public int OccasionTypeId { get; set; }
    }
}
