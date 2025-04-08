using MemApp.Domain.Core.Models;
using MemApp.Domain.Entities;
using MemApp.Domain.Entities.com;
using MemApp.Domain.Entities.Payment;
using Res.Domain.Entities.ROAPayment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Res.Domain.Entities
{
    public class MemberRegistrationInfo : BaseEntity
    {
        public string? ApplicationNo {  get; set; }
        public string? MemberShipNo {  get; set; }

        public string? Name {  get; set; }
        //public string? FatherName { get; set; }
        //public string? MotherName { get; set; }
        //public string? SpouseName { get; set; }
        //public string? HusbandName { get; set; }
        public string? NomineeName { get; set; }
        public string? InstituteNameBengali { get; set; }
        public string? InstituteNameEnglish { get; set; }
        public string? PhoneNo { get; set; }
        public string? PermenantAddress { get; set; }
        public string? MemberNID { get; set; }
        public string? MemberTINNo { get; set; }
        public string? MemberTradeLicense { get; set; }
        public DateTime? BusinessStartingDate { get; set; }
        public string? SignatureImgPath { get; set; }
        public string? NIDImgPath { get; set; }
        public string? TradeLicenseImgPath { get; set; }
        public string? TinImgPath { get; set; }

        public bool IsApproved { get; set; } =false;

       // public int? UserId { get; set; }
        public int? ApprovedBy { get; set; }
      //  public virtual User? User { get; set; }

     //   public virtual User? ApprovedBy { get; set; }

        public DateTime? ApproveTime {  get; set; }
        public DateTime? SignatureUploadingTime {  get; set; }

        public int? DivisionId { get; set; }

        public Division? Division { get; set; }
        public int? DistrictId { get; set; }
        public District? District { get; set; }
        public int? ThanaId { get; set; }
        public Thana? Thana { get; set; }

        public bool IsActive { get; set; }

        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? PasswordHash { get; set; }
        public string? PasswordSalt { get; set; }

        public bool IsFilled {  get; set; }=false;

        public Decimal? SubscriptionFee { get; set; }
        public Decimal? MembershipFee { get; set; }
        public string? ImgPath {  get; set; }
        public string? Note {  get; set; }

        public DateTime? PaidTill { get; set; }
        public DateTime? SubscriptionStarts { get; set; }

        public ICollection<RoSubscriptionDueTemp>? SubscriptionDuePayments { get; set; }
        public ICollection<RoaMembershipFeePayment>? MembershipFeePayments { get; set; }

        public ICollection<ROASubscriptionPayment>? SubscriptionPayments { get; set; }
        public ICollection<ROASubscriptionPaymentDetail>? SubscriptionPaymentDetails { get; set; }
        public ICollection<TopUp>? TopUps { get; set; }
      
    }
}
