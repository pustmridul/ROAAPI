using MemApp.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public decimal SubscriptionFee { get; set; }
        public decimal MembershipFee { get; set; }

        public string? Name { get; set; }
        //public string? FatherName { get; set; }
        //public string? MotherName { get; set; }
        //public string? SpouseName { get; set; }
        //public string? HusbandName { get; set; }
        public string? NomineeName { get; set; }
        public string? InstituteNameBengali { get; set; }
        public string? InstituteNameEnglish { get; set; }
        public string? PhoneNo { get; set; }
        public string? Email { get; set; }
        public string? PermenantAddress { get; set; }
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

        public string? DivisionName {  get; set; }
        public string? DistrictName {  get; set; }
        public string? ThanaName {  get; set; }
        public DateTime? SubscriptionStarts {  get; set; }
        public DateTime? PaidTill {  get; set; }

        public bool MemberFeePaid { get; set; }
    }

    public class MemberRegistrationInfoReq : MemberRegistrationInfoDto
    {
        public IFormFile? SignatureImgFile { get; set; }
        public IFormFile? NIDImgFile { get; set; }
        public IFormFile? TradeLicenseImgFile { get; set; }
        public IFormFile? TinImgFile { get; set; }
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
        public string? PermenantAddress { get; set; }
        public string? MemberNID { get; set; }
        public string? MemberTINNo { get; set; }
        public string? MemberTradeLicense { get; set; }
        public DateTime? BusinessStartingDate { get; set; }
       

        public bool? IsApproved { get; set; }
   

              

        public int? DivisionId { get; set; }
        public int? DistrictId { get; set; }
        public int? ThanaId { get; set; }

        
    }
}
