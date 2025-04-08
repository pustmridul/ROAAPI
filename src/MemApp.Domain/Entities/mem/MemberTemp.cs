using MemApp.Domain.Core.Models;
using MemApp.Domain.Entities.com;
using MemApp.Domain.Entities.Payment;

namespace MemApp.Domain.Entities.mem
{
    public class MemberTemp: BaseEntity
    {
        public string? Email { get; set; }
        public string? ClubName { get; set; }
        public string? CadetNo { get; set; }
        public string? BatchNo { get; set; }
        public string? CadetName { get; set; }
        public string FullName { get; set; }
        public string? Phone { get; set; }
        public string? Organaization { get; set; }
        public string? Designation { get; set; }
        public string? Specialization { get; set; }
        public string? OfficeAddress { get; set; }
        public string? HomeAddress { get; set; }
       
        public string? CardNo { get; set; }
        public string? MemberShipNo { get; set; }
        public string? PinNo { get; set; }

        public DateTime? Dob { get; set; }
        public int? CollegeId { get; set; }
        public College Colleges { get; set; }
        public MemberProfession MemberProfessions { get; set; }
        public int? MemberProfessionId { get; set; }
        public BloodGroup BloodGroup { get; set; }
        public int? BloodGroupId { get; set; }   
        public string? Spouse { get; set; }
        public string? SpouseOccupation { get; set; }
        public DateTime? Anniversary { get; set; }
        public bool IsActive { get; set; }
        public string? ImgFileUrl { get; set; }
        public string? Status { get; set; } = "Pending";

        public string? Nationality { get; set; }
        public DateTime? JoiningDate { get; set; }
        public DateTime? LeavingDate { get; set; }
        public string? CCCertificate { get; set; }
        public string? NID { get; set; }
        public string? TIN { get; set; }
        public string? HonorAndAwards { get; set; }
        public string? MotherName { get; set; }
        public string? FatherName { get; set; }
        public DateTime? SpouseDob { get; set; }
        public int? SpouseBloodGroupId { get; set; }
        public ICollection<MemberChildren>? MemberChildrens { get; set; }
        public string? HeightCms { get; set; }
        public string? WeightKgs { get; set; }
        public string? ColorOfEye { get; set; }
        public string? ColorOfHair { get; set; }
        public string? IdentificationMarks { get; set; }
        public string? Hobbies { get; set; }   
        public string? AncestralHome { get; set; }
        public string? PrimaryMember { get; set; }
        public string? AffiliateMember { get; set; }
        public string? ProfileImageUrl { get; set; }
        public DateTime? DateOfApplication { get; set; }
        public ICollection<MemberEducation>? MemberEducations { get; set; }

    }
}
