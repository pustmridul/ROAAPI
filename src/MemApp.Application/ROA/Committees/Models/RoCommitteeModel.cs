﻿using MemApp.Application.Extensions;
using ResApp.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResApp.Application.ROA.Committees.Models
{
    public class RoCommitteeModel
    {
    }

    public class RoCommitteeReq
    {
        public string Title { get; set; } = string.Empty;
        public string CommitteeDate { get; set; } = string.Empty;
        public string CommitteeType { get; set; } = string.Empty;
        public int? CommitteeCategoryId { get; set; }
        public int? DivisionId { get; set; }
        public int? DistrictId { get; set; }
        public int? ThanaId { get; set; }
        public int? ZoneId { get; set; }
        public int? MunicipalityId { get; set; }
        public int? UnionInfoId { get; set; }
        public int? WardId { get; set; }
        public string? CommitteeCategoryName { get; set; }
        public string? DivisionName { get; set; }
        public string? DistrictName { get; set; }
        public string? ThanaName { get; set; }
        public string? ZoneName { get; set; }
        public string? MunicipalityName { get; set; }
        public string? UnionName { get; set; }
        public string? WardName { get; set; }
        public bool IsActive { get; set; }
        public int Id { get; set; }
        public int CommitteeYear { get; set; }
        public List<RoCommitteeDetailReq> CommitteeDetails { get; set; } = new List<RoCommitteeDetailReq>();
    }
    public class RoCommitteeDetailReq
    {
        public int Id { get; set; }
        public int CommitteeId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string Designation { get; set; } = string.Empty;
        public string? MembershipNo { get; set; }
        public bool IsMasterMember { get; set; }
        public string? ImgFileUrl { get; set; }
    }
    public class RoCommitteeDD
    {
        public int CommitteeId { get; set; }
        public string CommitteeType { get; set; } = string.Empty;
        public string CommitteeTitle { get; set; } = string.Empty;
    }
    public class CommitteeDDListVm : Result
    {
        public long DataCount { get; set; }
        public List<RoCommitteeDD> DataList { get; set; } = new List<RoCommitteeDD>();
    }

    public class RoCommitteeVm : Result
    {
        public RoCommitteeReq Data { get; set; } = new RoCommitteeReq();
    }

    public class RoCommitteeListVm : Result
    {
        public long DataCount { get; set; }
        public List<RoCommitteeReq> DataList { get; set; } = new List<RoCommitteeReq>();
    }

    public class ExportRoCommitteeInfo
    {
        public string Title { get; set; } = string.Empty;
        public string CommitteeDate { get; set; } = string.Empty;
        public string CommitteeType { get; set; } = string.Empty;
        public string CommitteeCategoryName { get; set; } = string.Empty;

        public string? DivisionName { get; set; }
        public string? DistrictName { get; set; }
        public string? ThanaName { get; set; }
        public string? ZoneName { get; set; }
        public string? MunicipalityName { get; set; }
        public string? UnionName { get; set; }
        public string? WardName { get; set; }
        public int CommitteeYear { get; set; }
    }

    public class ExportRoCommitteeDetail
    {
        public int Id { get; set; }
        public int CommitteeId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string MembershipNo { get; set; } = string.Empty;
        public string IsMasterMember { get; set; } = string.Empty;
        public string ImgFileUrl { get; set; } = string.Empty;
        public bool HasImage { get; set; }
    }

    public class CommitteeSearchParam : PaginationParams
    {
        public int? Id { get; set; }
        public string? Type { get; set; }
        public int? Year { get; set; }
        public int? CategoryId { get; set; }

        public int? DivisionId { get; set; }
        public int? DistrictId { get; set; }
        public int? ThanaId { get; set; }

        public int? ZoneId { get; set; }
        public int? MunicipalityId { get; set; }
        public int? UnionInfoId { get; set; }
        public int? WardId { get; set; }


    }
}
