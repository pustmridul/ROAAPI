using MemApp.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Res.Domain.Entities
{
    public class MultipleOwner:BaseEntity
    {
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }

        public int? MemberId { get; set; }

        public MemberRegistrationInfo? Member { get; set; }
    }
}
