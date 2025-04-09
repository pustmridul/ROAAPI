using MemApp.Domain.Core.Models;
using MemApp.Domain.Entities.mem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Res.Domain.Entities.RoaCommittee
{
    public class RoCommitteeDetail : BaseEntity
    {
        public int CommitteeId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? ImgFileUrl { get; set; }
        public string Designation { get; set; } = string.Empty;
        public string? MembershipNo { get; set; }
      //  public bool IsMasterMember { get; set; }
        public RoCommittee Committee { get; set; }
    }
}
