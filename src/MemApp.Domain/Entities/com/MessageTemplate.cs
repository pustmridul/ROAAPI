using MemApp.Domain.Core.Models;

namespace MemApp.Domain.Entities.com
{
    public class MessageTemplate : BaseEntity
    {
        public int MessageTypeId { get; set; }
        public string Name { get; set; }= string.Empty;
        public string? Subject { get; set; }
        public string? Message { get; set; }
        public bool IsActive { get; set; }
        public int OccasionTypeId { get; set; }

    }
}
