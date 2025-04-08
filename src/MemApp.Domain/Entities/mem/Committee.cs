using MemApp.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Domain.Entities.mem
{
    public class Committee : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public DateTime CommitteeDate { get; set; }
        public string CommitteeType { get; set; }= string.Empty;
        public int? CommitteeCategoryId { get; set; }

        public bool IsActive { get; set; }
        public string CommitteeYear { get; set; } = string.Empty;
        public CommitteeCategory? CommitteeCategory { get; set; }
        public ICollection<CommitteeDetail> CommitteeDetails { get; set; }
    }
}
