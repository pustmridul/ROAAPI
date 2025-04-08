using MemApp.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Domain.Entities.mem
{
    public class MemberEducation : BaseEntity
    {
        public int MemberId { get; set; }
        public string? Board { get; set; }
        public string? Exam { get; set; }
        public string? Institution { get; set; }
        public string? PassingYear { get; set; }
        public string? Grade { get; set; }
        public MemberTemp MemberTemp { get; set; }
    }
}
