using MemApp.Application.Extensions;
using MemApp.Application.Mem.AreaLayouts.Models;
using MemApp.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.BoardMeetingMinuets.Models
{
    
    public class BoardMeetingMinuetDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime MeetingDate { get; set; }
        public string FileContent { get; set; } = string.Empty;
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
        public string? FileType { get; set; }
        public string? Note { get; set; }
        public bool IsActive { get; set; }
    }

   
}
