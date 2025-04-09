using MemApp.Domain.Core.Models;
using MemApp.Domain.Entities.mem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Res.Domain.Entities.RoaCommittee
{
    public class RoCommitteeCategory : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public ICollection<RoCommittee>? Committees { get; set; }
    }
}
