using MemApp.Application.Extensions;
using MemApp.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Committees.Models
{
    public class CommitteeModel
    {
    }
    public class CommitteeReq
    {
        public string Title { get; set; } = string.Empty;
        public string CommitteeDate { get; set; } = string.Empty;
        public string CommitteeType { get; set; } = string.Empty;
        public int? CommitteeCategoryId { get; set; }
        
        public string? CommitteeCategoryName { get; set; }
        public bool IsActive { get; set; }
        public int Id { get; set; }
        public string CommitteeYear { get; set; }=string.Empty;
        public List<CommitteeDetailReq> CommitteeDetails { get; set; }= new List<CommitteeDetailReq>();
    }
    public class CommitteeDetailReq
    {
        public int Id { get; set; }
        public int CommitteeId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string Designation { get; set; } = string.Empty;
        public string? MemberShipNo { get; set; }
        public bool IsMasterMember { get; set; }
        public string? ImgFileUrl { get; set; }
    }
   public class CommitteeDD
    {
        public int CommitteeId { get; set; }
        public string CommitteeType { get; set; } = string.Empty;
        public string CommitteeTitle { get; set; } = string.Empty;
    }
    public class CommitteeDDListVm : Result
    {
        public long DataCount { get; set; }
        public List<CommitteeDD> DataList { get; set; } = new List<CommitteeDD>();
    }

    public class CommitteeVm : Result
    {
       public CommitteeReq Data { get; set; } = new CommitteeReq();
    }

    public class CommitteeListVm : Result
    {
        public long DataCount { get; set; }
        public List<CommitteeReq> DataList { get; set; }= new List<CommitteeReq>();
    }

    public class ExportCommitteeInfo
    {
        public string Title { get; set; }= string.Empty;
        public string CommitteeDate { get; set; } = string.Empty;
        public string CommitteeType { get; set; } = string.Empty;
        public string CommitteeCategoryName { get; set; }=string.Empty;
        public string CommitteeYear { get; set; } = string.Empty;
    }

    public class ExportCommitteeDetail
    {
        public int Id { get; set; }
        public int CommitteeId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public string Phone { get; set; }=string.Empty ;
        public string Email { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string MemberShipNo { get; set; } = string.Empty;
        public string IsMasterMember { get; set; } = string.Empty;
        public string ImgFileUrl { get; set; } = string.Empty;
        public bool HasImage { get; set; }
    }

    public class CommitteeCatReq
    {
        public string Title { get; set; } = string.Empty;
        public int Id { get; set; }
    }
}
