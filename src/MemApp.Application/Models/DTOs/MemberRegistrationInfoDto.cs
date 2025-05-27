using Microsoft.AspNetCore.Http;
using Res.Domain.Entities;
using System.Text.Json.Serialization;

namespace ResApp.Application.Models.DTOs
{
    public class MemberRegistrationInfoDto
    {
        public int Id { get; set; }
        public int CreatedBy { get; set; }
        public string? CreatedByName { get; set; }

        public DateTime? CreatedOn { get; set; }
        public int? LastModifiedBy { get; set; }
        public string? LastModifiedByName { get; set; }

        public DateTime? LastModifiedOn { get; set; }

        public string? ApplicationNo { get; set; }
        public string? MemberShipNo { get; set; }

        public decimal? SubscriptionFee { get; set; }
        public decimal? MembershipFee { get; set; }

        public string? Name { get; set; }
      
        public string? NomineeName { get; set; }
        public string? InstituteNameBengali { get; set; }
        public string? InstituteNameEnglish { get; set; }
        public string? PhoneNo { get; set; }
        public string? Email { get; set; }
        public string? PermanentAddress { get; set; }
        public string? MemberNID { get; set; }
        public string? MemberTINNo { get; set; }
        public string? MemberTradeLicense { get; set; }
        public DateTime? BusinessStartingDate { get; set; }
        public string? SignatureImgPath { get; set; }
        public string? NIDImgPath { get; set; }
        public string? TradeLicenseImgPath { get; set; }
        public string? TinImgPath { get; set; }
        public string? ProfileImgPath { get; set; }

        public bool IsApproved { get; set; } = false;
        public bool IsFilled { get; set; } = false;

        public virtual User? ApprovedBy { get; set; }

        public DateTime? ApproveTime { get; set; }
        public DateTime? SignatureUploadingTime { get; set; }

        public int? DivisionId { get; set; }
        public int? DistrictId { get; set; }
        public int? ThanaId { get; set; }

        public int? ZoneId { get; set; }
        public int? MunicipalityId { get; set; }
        public int? UnionInfoId { get; set; }
        public int? WardId { get; set; }
        public int? MemberCategoryId { get; set; }
        public string? MemberCategoryName { get; set; }


        public string? DivisionName {  get; set; }
        public string? DistrictName {  get; set; }
        public string? ThanaName {  get; set; }
        public DateTime? SubscriptionStarts {  get; set; }
        public DateTime? PaidTill {  get; set; }

        public bool MemberFeePaid { get; set; }

        public List<ContactDetailReq>? ContactDetailReq { get; set; }
    }

    public class MemberRegistrationInfoReq : MemberRegistrationInfoDto
    {
        public IFormFile? SignatureImgFile { get; set; }
        public IFormFile? NIDImgFile { get; set; }
        public IFormFile? TradeLicenseImgFile { get; set; }
        public IFormFile? TinImgFile { get; set; }

        public IFormFile? ProfileImg { get; set; }

        public string? EmailId { get; set; }


        [JsonIgnore]
        public string? WebRootPath { get; set; }
    }

    public class MemberSearchParam : PaginationParams
    {
        public int Id { get; set; }
      
        public string? ApplicationNo { get; set; }
        public string? MemberShipNo { get; set; }

        public string? Name { get; set; }
        public string? Email { get; set; }
       
        public string? NomineeName { get; set; }
        public string? InstituteNameBengali { get; set; }
        public string? InstituteNameEnglish { get; set; }
        public string? PhoneNo { get; set; }
        public string? PermanentAddress { get; set; }
        public string? MemberNID { get; set; }
        public string? MemberTINNo { get; set; }
        public string? MemberTradeLicense { get; set; }
        public DateTime? BusinessStartingDate { get; set; }
       

        public bool? IsApproved { get; set; }
   

              

        public int? DivisionId { get; set; }
        public int? DistrictId { get; set; }
        public int? ThanaId { get; set; }

        public int? ZoneId { get; set; }
        public int? MunicipalityId { get; set; }
        public int? UnionInfoId { get; set; }
        public int? WardId { get; set; }

    }

    public class ContactDetailReq
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }
}
