using MemApp.Application.Core.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Models;
using System.Security.Principal;

namespace MemApp.Application.Mem.Members.Models
{
    public class MemberModel
    {

    }
    #region member Type
    public class MemberTypeDto
    {
        public string Name { get; set; }=string.Empty;
        public int Id { get; set; }
        public string? CategoryName {  get; set; }
        public int CategoryPatternId { get; set; }
        public bool IsSubscribed { get; set; }
        public string? OldId { get; set; }

    }
    
   
    #endregion
    #region member status
    public class MemberStatusDto
    {
        public string Name { get; set; } = string.Empty;
        public int Id { get; set; }
    }
    
    
    #endregion


    #region Member Active Status
    public class MemberActiveStatusDto
    {
        public string Name { get; set; } = string.Empty;
        public int Id { get; set; }
    }
    
    
    #endregion
    #region Member Address
    public class MemberAddressDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
   
    
    #endregion

    #region member Profession
    public class MemberProfessionDto
    {
        public string Name { get; set; } = string.Empty;
        public int Id { get; set; }
    }
    
    
    #endregion

    #region MemberRegistration


    public class MemberFeeReq
    {
        public int MemberId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal PaymentAmount { get; set; }
        public List<MemberFeesMapReq> MemberFeesMapReqs { get; set; } = new List<MemberFeesMapReq>();   
    }

    public class MemberFamilyReq
    {
        public int MemberId { get; set; }
        public string? CardNo { get; set; }
        public string? Spouse { get; set; }
        public string? SpouseOccupation { get; set; }
        public string? Anniversary { get; set; }
        public string? Nok { get; set; }
        public List<MemberChildrenReq> MemberchildrenReqs { get; set; } = new List<MemberChildrenReq>();

    }
    public class MemberRes
    {
        public string? ImgFileUrl { get; set; }
        public int Id { get; set; }
        public string? MemberShipNo { get; set; }
        public string? CardNo { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string TypeText { get; set; } = string.Empty;
        public string StatusText { get; set; } = string.Empty;
        public string? ActiveStatusText { get; set; }

        public string Name { get; set; } = string.Empty;
        public string? CollegeCode { get; set; }
        public string CollegeName { get; set; } = string.Empty;
        public string? CadetName{ get; set; }
        public string? CadetNo { get; set; }
        public string? BatchNo { get; set; }

        public string? HscYear { get; set; }
        public string BloodGroupText { get; set; } = string.Empty;
        public DateTime? Dbo { get; set; }
        public string ProfessionText { get; set; } = string.Empty;
        public string? Designation { get; set; }
        public bool IsActive { get; set; }
        public bool? IsMasterMember { get; set; }
        public string MemberFullId { get; set; } = string.Empty;

        public DateTime? JoinDate { get; set; }
        public DateTime? LeaveDate { get; set; }

        public string? PermanentAddress { get; set; }
        public string? NID { get; set; }
        public string? EmergencyContact { get; set; }
        public DateTime? Anniversary { get; set; }

    }

    public class MemberResList : Result
    {
        public long DataCount { get; set; }
        public List<MemberRes> DataList { get; set; }= new List<MemberRes>();
    }

    public class RegisterMemberRes
    {
        public string? PostalAddress { get; set; }
        public int Id { get; set; }
        public string MemberTypeText { get; set; } = string.Empty;
        public string? PrvCusID { get; set; }
        public string? CusCategory { get; set; }

        public string? Title { get; set; }

        public string? CusName { get; set; }
        public string? QBCusName { get; set; }


        public string? City { get; set; }
        public bool? IsMasterMember { get; set; }

        public string? Email { get; set; }

        public string? CusProfession { get; set; }

        public string? DiscAllowed { get; set; }

        public decimal? DiscPrcnt { get; set; }

        public decimal? CreditLimit { get; set; }

        public decimal? CreditDays { get; set; }

        public string? Mrcode { get; set; }

        public string? Active { get; set; }

        public string? Opening { get; set; }

        public string? ClubName { get; set; }
        public int? MemberStatusId { get; set; }
        public string? MemberStatusText { get; set; }

        public int? MemberActiveStatusId { get; set; }
        public string? MemberActiveStatusText { get; set; }

        public string CadetNo { get; set; } = string.Empty;
        public string BatchNo { get; set; } = string.Empty;
        public string MemberFullId { get; set; } = string.Empty;
        public string? PaidTill { get; set; }
        public string? CadetName { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Organaization { get; set; }
        public string? Designation { get; set; }
        public string? Specialization { get; set; }
        public string? OfficeAddress { get; set; }
        public string? HomeAddress { get; set; }
        public string? Dob { get; set; }
        public int MemberTypeId { get; set; }
        public int CollegeId { get; set; }
        public string CollegeName { get; set; } = string.Empty;
        public string? CollegeCode { get; set; }
        public int? MemberProfessionId { get; set; }
        public string MemberProfessionText { get; set; } = string.Empty;
        public int? BloodGroupId { get; set; }
        public string BloodGroupText { get; set; } = string.Empty;
        public string? HscYear { get; set; }
        public string? Spouse { get; set; }
        public string? SpouseOccupation { get; set; }
        public string? Anniversary { get; set; }
        public string? PinNo { get; set; }
        public string? CardNo { get; set; }
        public string? MemberShipNo { get; set; }
        public string? ExpireDate { get; set; }
        public Decimal CurrentBalance { get; set; }
        public string? ImgFileUrl { get; set; }
        public string? ActiveStatusDate { get; set; }
        public string? Nok { get; set; }
        public string? DeviceId { get; set; }
        public bool HasSubscription { get; set; }

        public string? JoinDate { get; set; }
        public string? LeaveDate { get; set; }


        public string? PermanentAddress { get; set; }
        public string? NID { get; set; }
        public string? EmergencyContact { get; set; }
        public string? ClubJoinDate { get; set; }

        public SpouseRegistrationRes? SpouseData { get; set; }
        public List<MemberChildrenReq>? MemberchildrenReqs { get; set; } = new List<MemberChildrenReq>();
        public List<MemberFeesMapRes>? MemberFeesMapRess { get; set; }=new List<MemberFeesMapRes>();
    }

    public class RegisterMemberVm : Result
    {
        public RegisterMemberRes Data { get; set; } = new RegisterMemberRes();
    }
    public class RegisterMemberListVm : Result
    {
        public long DataCount { get; set; }
        public List<RegisterMemberRes> DataList { get; set; } = new List<RegisterMemberRes>();
    }

    public class ColumnFilter
    {
        public string ColumnName { get; set; } = string.Empty;
        public string FilterValue { get; set; } = string.Empty;
    }

    public class MemberSearchReq
    {
        public string? MemberShipNo { get; set; }
        public string? CadetName { get; set; }
        public string? FullName { get; set; }
        public int? MemberTypeId { get; set; }
        public int? MemberActiveStatusId { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int? CollegeId { get; set; }
        public string? BatchNo { get; set; }
        public int? BloodGroupId { get; set; }
        public string? Organaization { get; set; }
        public string? Designation { get; set; }
        public string? Specialization { get; set; }
        public int? MemberProfessionId { get; set; }
        public string? Dob { get; set; }
        public string? DoA { get; set; }
        public string? HscYear { get; set; }
        public string? CadetNo { get; set; }
        public string? memFullId { get; set; }
        public string queryString { get; set; } = string.Empty;
        public string? Spouse { get; set; }
        public string? Children { get; set; }


    }
    public class MemberSearchRes
    {
        public string ImgFileUrl { get; set; } = string.Empty;
        public int? Id { get; set; }
        public string? MemberShipNo { get; set; }
        public string? ContactName { get; set; }
        public string? FullName { get; set; }
        public int? MemberTypeId { get; set; }
        public int? MemberActiveStatusId { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int? CollegeId { get; set; }
        public string? BatchNo { get; set; }
        public int? BloodGroupId { get; set; }
        public string? Organaization { get; set; }
        public string? Designation { get; set; }
        public string? Specialization { get; set; }
        public int? MemberProfessionId { get; set; }
        public string? Dob { get; set; }
        public string? HscYear { get; set; }
        public string CardNo { get; set; } = string.Empty;
        public string MemberFullId { get; set; } = string.Empty;
        public string CadetNo { get; set; } = string.Empty;

    }
    public class MemberSearchVm : Result
    {
        public MemberSearchRes Data { get; set; } = new MemberSearchRes();
    }
    public class MemberSearchListVm: Result
    {
        public int DataCount { get; set; }
        public List<MemberSearchRes> DataList { get; set; } = new List<MemberSearchRes>();
    }


    public class MemberUpdateVm : Result
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Organaization { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string OfficeAddress { get; set; } = string.Empty;
        public string HomeAddress { get; set; } = string.Empty;
        public string? Dob { get; set; }
        public string Spouse { get; set; } = string.Empty;
        public string SpouseOccupation { get; set; } = string.Empty;
    }
    //
    public class ExportMember
    {
        public string ImgFileUrl { get; set; } = string.Empty;
        public int Id { get; set; }
        public string MemberShipNo { get; set; } = string.Empty;
        public string CardNo { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string TypeText { get; set; } = string.Empty;
        public string StatusText { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string CollegeCode { get; set; } = string.Empty;
        public string CollegeName { get; set; } = string.Empty;
        public string CadetName { get; set; } = string.Empty;
        public string HscYear { get; set; } = string.Empty;
        public string BloodGroupText { get; set; } = string.Empty;
        public string Dbo { get; set; } = string.Empty;
        public string ProfessionText { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsMasterMember { get; set; }
        public string MemberFullId { get; set; } = string.Empty;

        public string JoinDate { get; set; } = string.Empty;
        public string LeaveDate { get; set; } = string.Empty;

        public string PermanentAddress { get; set; } = string.Empty;
        public string NID { get; set; } = string.Empty;
        public string EmergencyContact { get; set; } = string.Empty;

    }




    #endregion

    #region MemberChildren
    public class MemberChildrenReq
    {
        public int Id { get; set; }
        public string CadetNo { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string ContactName { get; set; } = string.Empty;
        public string? Dob { get; set; }
        public string Gender { get; set; } = string.Empty;
        public int? RegisterMemberId { get; set; }
    }
    #endregion

    #region CategoryPatterns
    public class CategoryPatternsDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
    }
   

    #endregion

    #region MemberShipFee
    public class MemberShipFeeDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public double Amount { get; set; }
        public string? DisplayName { get; set; }
        public int MemberTypeId { get; set; }
        public string? MemberTypeText { get; set; }
        public bool IsCurrent { get; set; }
        public DateTime? LastUpdated { get; set; }
        public bool IsChecked { get; set; }

    }

   
    #endregion

    #region MemberFeesMap

    public class MemberFeesMapReq
    {
        public int Id { get; set; }
        public int MemberShipFeeId { get; set; }
        public string MemberFeesTitle { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public int? RegisterMemberId { get; set; }
    }

    public class MemberFeesMapRes
    {
        public int Id { get; set; }
        public int MemberShipFeeId { get; set; }
        public bool IsChecked { get; set; }
        public string MemberFeesTitle { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
    #endregion
    #region
    public class SpouseRegistrationReq
    {
        public string PrvCusID { get; set; } = string.Empty;
        public int Id { get; set; }
        public string SpouseCardNo { get; set; } = string.Empty;
        public string CadetNo { get; set; } = string.Empty;
        public string BatchNo { get; set; } = string.Empty;
        public string MemberShipNo { get; set; } = string.Empty;
        public string MemberFullId { get; set; } = string.Empty;
        public string? PaidTill { get; set; }
        public string ContactName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Organaization { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string OfficeAddress { get; set; } = string.Empty;
        public string HomeAddress { get; set; } = string.Empty;
        public string? Dob { get; set; }
        public int MemberActiveStatusId { get; set; }
        public int MemberStatusId { get; set; }
        public int MemberTypeId { get; set; }
        public int CollegeId { get; set; }
        public int MemberAddressID { get; set; }
        public int MemberProfessionID { get; set; }
        public int BloodGroupId { get; set; }
        public string HscYear { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PinNo { get; set; } = string.Empty;
        public string? Picture { get; set; }
    }

    public class SpouseRegistrationRes
    {
        public string? PrvCusID { get; set; }
        public int Id { get; set; }
        public string? CardNo { get; set; }
        public string? CadetNo { get; set; }
        public string? BatchNo { get; set; }
        public string MemberShipNo { get; set; } = string.Empty;
        public string MemberFullId { get; set; } = string.Empty;
        public string? PaidTill { get; set; }
        public string? ContactName { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Organaization { get; set; }
        public string? Designation { get; set; }
        public string? Specialization { get; set; }
        public string? OfficeAddress { get; set; }
        public string? HomeAddress { get; set; }
        public string? Dob { get; set; }
        public int MemberActiveStatusId { get; set; }
        public int MemberTypeId { get; set; }
        public int? CollegeId { get; set; }
        public int? MemberProfessionId { get; set; }
        public int? BloodGroupId { get; set; }
        public string? HscYear { get; set; }
        public string? Email { get; set; }
        public string? PinNo { get; set; }
        public string? ImgFileUrl { get; set; }
        public string? Spouse { get; set; }
        public string? SpouseOccupation { get; set; }
        public string? Anniversary { get; set; }
    }


    public class MemberFileVm : Result
    {
        public string Title { get; set; } = string.Empty;   
        public string FileData { get; set; } = string.Empty;

    }
    public class MemberFileListVm
    {
        public int DataCount { get; set; }
        public List<MemberFileVm> DataList { get; set; } = new List<MemberFileVm>(); 
    }
    #endregion

    public class ViewMemberDto
    {
        public string CardNo { get; set; } = string.Empty;
        public string MembershipNo { get; set; } = string.Empty;
        public string IsMasterMember { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Organaization { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string MemberFullId { get; set; }=string.Empty;
        public string BatchNo { get; set; } = string.Empty;
        public string CadetNo { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string OfficeAddress { get; set; } = string.Empty;
        public string HomeAddress { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Dob { get; set; } = string.Empty;
        public string HscYear { get; set; } = string.Empty;
        public string Spouse { get; set; } = string.Empty;
        public string Anniversary { get; set; } = string.Empty;
        public string Children { get; set; } = string.Empty;
        public string ChildrenDOB { get; set; } = string.Empty;
        public string SpouseOccupation { get; set; } = string.Empty;
        public string MemberActiveStatusId { get; set; } = string.Empty;
        public string ActiveStatus { get; set; } = string.Empty;
        public string CollegeName { get; set; } = string.Empty;
        public string BLoodGroup { get; set; } = string.Empty;
        public string MemberType { get; set; } = string.Empty;
        public string MemberStatus { get; set; } = string.Empty;
        public string MemberProfession { get; set; } = string.Empty;
        public string PaidTill { get; set; } = string.Empty;
        public string PinNo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;


    }

    public class MemberPhoneReq
    {
        public int Id { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public string MemberShipNo { get; set; } = string.Empty;
        public string PhoneNo { get; set; } = string.Empty;
        public string MemberEmail { get; set; } = string.Empty;

    }
    public class MemberPhoneListVm :Result
    {
        public int DataCount { get; set; }
        public List<MemberPhoneReq> DataList { get; set; } = new List<MemberPhoneReq>();
    }

    public class ReportDto
    {
        public string MembershipNo1 { get; set; } = string.Empty;
        public string FullName1 { get; set; } = string.Empty;
        public string HomeAddress1 { get; set; } = string.Empty;

        public string MembershipNo2 { get; set; } = string.Empty;
        public string FullName2 { get; set; } = string.Empty;
        public string HomeAddress2 { get; set; } = string.Empty;
    }

}
