using MemApp.Application.Extensions;
using MemApp.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.MemberEducations.Models
{
    public class MemberEducationModel
    {
    }
    public class MemberEducationeReq
    {
        public string? PrvCusID { get; set; }
        public string? Board { get; set; }
        public string? Exam { get; set; }
        public string? Institution { get; set; }
        public string? PassingYear { get; set; }
        public string? Grade { get; set; }

        public int Id { get; set; }
    }
    public class MemberEducationeRes
    {
        public string? PrvCusID { get; set; }
        public string? Board { get; set; }
        public string? Exam { get; set; }
        public string? Institution { get; set; }
        public string? PassingYear { get; set; }
        public string? Grade { get; set; }

        public int Id { get; set; }
    }
    public class MemberEducationeVm: Result
    {
       public MemberEducationeRes Data { get; set; } = new MemberEducationeRes();
    }

    public class MemberEducationeListVm :Result
    {
        public long DataCount { get; set; }
        public List<MemberEducationeRes> DataList { get; set; }= new List<MemberEducationeRes>();
    }
}
