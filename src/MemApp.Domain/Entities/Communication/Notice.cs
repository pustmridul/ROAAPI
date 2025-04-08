using MemApp.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Domain.Entities.Communication
{
    public class Notice:BaseEntity
    {
        public string Title { get; set; }
        public int? AttachmentType { get; set; }
        public string? TextContent { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? FileUrl { get; set; }
        public bool IsActive { get; set; }
    }
}
