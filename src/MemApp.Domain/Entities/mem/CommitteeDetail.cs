using MemApp.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Domain.Entities.mem
{
    public class CommitteeDetail : BaseEntity
    {
        public int CommitteeId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? ImgFileUrl { get; set; }
        public string Designation { get; set; } = string.Empty;  
        public string? MemberShipNo { get; set; }
        public bool IsMasterMember { get; set; }
        public Committee Committee { get; set; }
       
    }
}
