using MemApp.Application.Extensions;
using Microsoft.AspNetCore.Http;


namespace MemApp.Application.Mem.Communication.Models
{
    public class NoticeModelDto
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public int? AttachmentType { get; set; }
        public string? TextContent { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? FileUrl { get; set; }
        public IFormFile? File { get; set; }
    }


    
    
}
