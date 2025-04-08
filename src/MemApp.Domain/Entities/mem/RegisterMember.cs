using MemApp.Domain.Core.Models;
using MemApp.Domain.Entities.com;
using MemApp.Domain.Entities.Payment;
using MemApp.Domain.Entities.Sale;

namespace MemApp.Domain.Entities.mem
{
    public class RegisterMember : BaseEntity
    {

        public string? PrvCusID { get; set; }
        public string? CusCategory { get; set; }
        public string? Title { get; set; }
        public string? CusName { get; set; }
        public string? Address { get; set; }
        public string? QBCusName { get; set; }
        public string? City { get; set; }
        public string? Email { get; set; }
        public string? CusProfession { get; set; }
        public string? DiscAllowed { get; set; }
        public decimal? DiscPrcnt { get; set; }
        public decimal? CreditLimit { get; set; }
        public decimal? CreditDays { get; set; }
        public string? Mrcode { get; set; }
        public string? Active { get; set; }
        public DateTime? Opening { get; set; }
        public string? ClubName { get; set; }
        public string? CardNo { get; set; }
        public string? PinNo { get; set; }
        public string? PinNoHash { get; set; }
        public string? PinNoSalt { get; set; }
        public string? MembershipNo { get; set; }
        public string? Picture { get; set; }
        public DateTime? ExpireDate { get; set; }
        public bool? IsMasterMember { get; set; }
        public string? AccountNumber { get; set; }
        public string? CadetNo { get; set; }
        public string? BatchNo { get; set; }
        public string MemberFullId { get; set; } = string.Empty;
        public DateTime? PaidTill { get; set; }
        public string? CadetName { get; set; }
        public string FullName { get; set; }=string.Empty;
        public string? Phone { get; set; }
        public string? Organaization { get; set; }
        public string? Designation { get; set; }
        public string? Specialization { get; set; }
        public string? OfficeAddress { get; set; }
        public string? HomeAddress { get; set; }
        public DateTime? Dob { get; set; }
        public MemberStatus? MemberStatus { get; set; }
        public int? MemberStatusId { get; set; }
        public MemberActiveStatus? MemberActiveStatus { get; set; }
        public int? MemberActiveStatusId { get; set; }
        public MemberType MemberTypes { get; set; }
        public int MemberTypeId { get; set; }
        public College Colleges { get; set; }
        public int CollegeId { get; set; }
        public MemberProfession? MemberProfessions { get; set; }
        public int? MemberProfessionId { get; set; }
        public BloodGroup? BloodGroup { get; set; }
        public int? BloodGroupId { get; set; }
        public ICollection<MemberChildren> MemberChildrens { get; set; }
        public ICollection<MemberFeesMap> MemberFeesMaps { get; set; }
        public ICollection<SubscriptionPayment> SubscriptionPayments { get; set; }
        public ICollection<SubscriptionPaymentDetail> SubscriptionPaymentDetails { get; set; }
        //public ICollection<TopUp> TopUps { get; set; }
        public string? HscYear { get; set; }
        public string? Spouse { get; set; }
        public string? SpouseOccupation { get; set; }
        public DateTime? Anniversary { get; set; }
        public bool IsActive { get; set; }
        public Decimal CurrentBalance { get; set; }
        public int? MemberId { get; set; }
        public DateTime? ActiveStatusDate { get; set; }
        public string? Nok { get; set; }
        public string? ImgFileUrl { get; set; }
        public string? DeviceId { get; set; }
       //new added
        public DateTime? JoinDate { get; set; }
        public DateTime? LeaveDate { get; set; }


        public string? PermanentAddress { get; set; }
        public string? NID { get; set; }
        public string? EmergencyContact { get; set; }

        public DateTime? ClubJoinDate { get; set; }


        public ICollection<ServiceSale>? ServiceSale { get; set; }
        public ICollection<SubscriptionPaymentTemp>? SubscriptionPaymentTemp { get; set; }

        public ICollection<MiscSale>? MiscSale { get; set; }

        public ICollection<Donate>? Donate { get; set; }



    }
}
