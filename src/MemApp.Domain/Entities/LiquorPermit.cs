using MemApp.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Domain.Entities
{
    public class LiquorPermit:BaseEntity
    {
        public int MemberId { get; set; }
        public string? Title { get; set; }
        public string? FileUrl { get; set; }
        public bool IsActive { get; set; }
    }
}
