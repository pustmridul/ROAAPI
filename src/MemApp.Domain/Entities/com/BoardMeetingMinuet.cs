using MemApp.Domain.Core.Models;
using MemApp.Domain.Entities.mem;

namespace MemApp.Domain.Entities.com
{
    public class BoardMeetingMinuet : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public DateTime MeetingDate { get; set; }
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
        public string? FileType { get; set; }
        public string? FileSize { get; set; }
        public string? Note { get; set; }
        public bool IsActive { get; set; }
        public Byte[]? FileContent { get; set; }

        public int? CommitteeId { get; set; }
        public string? CommitteeTitle { get; set; }

    }
}
