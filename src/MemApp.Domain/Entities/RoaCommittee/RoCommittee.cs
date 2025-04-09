using MemApp.Domain.Core.Models;
using MemApp.Domain.Entities.mem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Res.Domain.Entities.RoaCommittee
{
    public class RoCommittee : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public DateTime CommitteeDate { get; set; }
        public string CommitteeType { get; set; } = string.Empty;
        public int? CommitteeCategoryId { get; set; }
        public int? DivisionId { get; set; }
        public int? DistrictId { get; set; }
        public int? ThanaId { get; set; }

        public bool IsActive { get; set; }
        public int CommitteeYear { get; set; }
        public RoCommitteeCategory? CommitteeCategory { get; set; }
        public Division? Division { get; set; }
        public District? District { get; set; }
        public Thana? Thana { get; set; }
        public ICollection<RoCommitteeDetail> CommitteeDetails { get; set; }
    }
}
