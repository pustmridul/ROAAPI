using MemApp.Application.Extensions;
using MemApp.Application.Mem.Members.Models;
using MemApp.Application.Models;
using MemApp.Domain.Core.Models;
using MemApp.Domain.Entities.com;
using MemApp.Domain.Entities.mem;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.MemberTemps.Models
{
    public class MemberTempModel
    {
    }
    public class MemberTempReq
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Dob { get; set; }
        public int? BloodGroupId { get; set; }
        public string? Nationality { get; set; }
        public int? CollegeId { get; set; }
        public string? BatchNo { get; set; }
        public string? CadetNo { get; set; }
        public string? JoiningDate { get; set; }
        public string? LeavingDate { get; set; }
        public string? CCCertificate { get; set; }
        public string? NID { get; set; }
        public string? TIN { get; set; }
        public string? Organaization { get; set; }
        public string? Designation { get; set; }
        public string? HonorAndAwards { get; set; }
        public string? MotherName { get; set; }
        public string? FatherName { get; set; }
        public string? Spouse { get; set; }
        public string? SpouseDob { get; set; }
        public string? Anniversary { get; set; }
        public int? SpouseBloodGroupId { get; set; }
        public string? SpouseOccupation { get; set; }
        public int? ChildrenId { get; set; }
        public string? HeightCms { get; set; }
        public string? WeightKgs { get; set; }
        public string? ColorOfEye { get; set; }
        public string? ColorOfHair { get; set; }
        public string? IdentificationMarks { get; set; }
        public string? Hobbies { get; set; }
        public string? OfficeAddress { get; set; }
        public string? HomeAddress { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? AncestralHome { get; set; }
        public string? PrimaryMember { get; set; }
        public string? AffiliateMember { get; set; }
        public string? DateOfApplication { get; set; }
        public string? ClubName { get; set; }
        public string? CadetName { get; set; }
        public string? Specialization { get; set; }
        public int? MemberProfessionId { get; set; }
        public bool? IsActive { get; set; }
        
        public string? ImgFileUrl { get; set; }
        public string? Status { get; set; }

        [JsonIgnore]
        public string? CardNo { get; set; }
        [JsonIgnore]
        public string? MemberShipNo { get; set; }
        public string? PinNo { get; set; }
        public string? DesignationAndOrganization { get; set; }
        public List<MemberChildrenReq>? MemberChildrenReqs { get; set; }
        public List<MemberEducationReq>? MemberEducationReqs { get; set; }
        [JsonIgnore]
        public string? IpAddress { get; set; }
        public string? AppId { get; set; }
        public IFormFile? ProfileImage { get; set; }
        public string? ProfileImageUrl { get; set; }


    }


    public class MemberTempRes
    {
        public int Id { get; set; }
        public string FullName { get; set; }= string.Empty;
        public string? Dob { get; set; }
        public int BloodGroupId { get; set; }
        public string Nationality { get; set; } = string.Empty;
        public int CollegeId { get; set; }
        public string BatchNo { get; set; } = string.Empty;
        public string CadetNo { get; set; } = string.Empty;
        public string? JoiningDate { get; set; }
        public string? LeavingDate { get; set; }
        public string CCCertificate { get; set; } = string.Empty;
        public string NID { get; set; }=string.Empty;
        public string TIN { get; set; } = string.Empty;
        public string Organaization { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string HonorAndAwards { get; set; } = string.Empty;
        public string MotherName { get; set; } = string.Empty;
        public string FatherName { get; set; } = string.Empty;
        public string Spouse { get; set; } = string.Empty;
        public string? SpouseDob { get; set; }
        public string? Anniversary { get; set; }
        public int SpouseBloodGroupId { get; set; }
        public string SpouseOccupation { get; set; } = string.Empty;
        public int ChildrenId { get; set; }
        public string HeightCms { get; set; } = string.Empty;
        public string WeightKgs { get; set; } = string.Empty;
        public string ColorOfEye { get; set; } = string.Empty;
        public string ColorOfHair { get; set; } = string.Empty;
        public string IdentificationMarks { get; set; } = string.Empty;
        public string Hobbies { get; set; } = string.Empty;
        public string OfficeAddress { get; set; } = string.Empty;
        public string HomeAddress { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string AncestralHome { get; set; } = string.Empty;
        public string PrimaryMember { get; set; } = string.Empty;
        public string AffiliateMember { get; set; } = string.Empty;
        public string? DateOfApplication { get; set; }
        public string ClubName { get; set; } = string.Empty;
        public string CadetName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public int MemberProfessionId { get; set; }
        public bool IsActive { get; set; }

        public string ImgFileUrl { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string DesignationAndOrganization { get; set; } = string.Empty;
        public List<MemberChildrenReq>? MemberChildrenReqs { get; set; }
        public List<MemberEducationReq>? MemberEducationReqs { get; set; }

    }
    public class MemberTempVm: Result
    {
       public MemberTempReq Data { get; set; } = new MemberTempReq();
    }

    public class MemberTempListVm : Result
    {
        public long DataCount { get; set; }
        public List<MemberTempRes> DataList { get; set; }= new List<MemberTempRes>();
    }

    public class MemberEducationReq
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public string? Board { get; set; }
        public string? Exam { get; set; }
        public string? Institution { get; set; }
        public string? PassingYear { get; set; }
        public string? Grade { get; set; }
    }
}
