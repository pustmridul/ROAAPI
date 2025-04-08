using MemApp.Domain.Core.Models;

namespace MemApp.Domain.Entities.mem
{
    public class MemFiles : BaseEntity
    {
        public string? Titile { get; set; }
        public string? FileContent { get; set; }
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
        public string? FileSize { get; set; }
        public int? MemberId { get; set; }
        public string? FileType { get; set; }
        public bool IsActive { get; set; }
        public Byte[]? MemImage { get; set; } 
    }
}
