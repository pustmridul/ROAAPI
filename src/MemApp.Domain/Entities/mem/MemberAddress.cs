using MemApp.Domain.Core.Models;

namespace MemApp.Domain.Entities.mem
{
    public class MemberAddress : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Code { get; set; }
        public string? Type { get; set; }
        public bool IsActive { get; set; }

        public int MemberId { get; set; }
    }
}
