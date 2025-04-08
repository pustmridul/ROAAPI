using MemApp.Application.Extensions;
using MemApp.Application.Models;
using MemApp.Application.Models.Responses;
using MemApp.Domain.Entities.com;
using MemApp.Domain.Entities.mem;
using System.Text.Json.Serialization;

namespace MemApp.Application.App.Models
{
    public class MemberShipInfo
    {
    }
    #region MemberInfo
    public class MemberInfoReq
    {
        public string MemberShipNo { get; set; } = string.Empty;
    }
    public class MemberInfoRes
    {
        public int Id { get; set; }
        public string MemberShipNo { get; set; } = string.Empty;
        public string CardNo { get; set; } = string.Empty;
        public string ImgFileUrl { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;    
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class MemberInfoVm : Result
    {
        public MemberInfoRes Data { get; set; } = new MemberInfoRes();
    }

    public class MemberInfoListVm : Result
    {
        public int DataCount { get; set; }
        public List<MemberInfoRes> DataList { get; set; } = new List<MemberInfoRes>();
    }
    #endregion

     #region LoginMember
    public class MemberLoginReq
    {
        public string CardNo { get; set; } = string.Empty;  
        public string PinNo { get; set; } = string.Empty;
        public string AppId { get; set; } = string.Empty;
        [JsonIgnore]
        public string IpAddress { get; set; }=string.Empty;
        public string? DeviceToken { get; set; }
    }
    public class MemberLoginRes 
    {
        public string ImgFileUrl { get; set; } = string.Empty;
        public int MemberId { get; set; }
        public string? CardNo { get; set; }    
        public string? MembershipNo { get; set; }
        public string? Picture { get; set; }
        public DateTime? ExpireDate { get; set; }
        public bool? IsMasterMember { get; set; }
        public string? AccountNumber { get; set; }
        public decimal CurrentBalance { get; set; }
        public string? PaidTill { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Organaization { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string Specialaization { get; set; } = string.Empty;
        public string? OfficeAddress { get; set; }
        public string? HomeAddress { get; set; }

        public string? Dob { get; set; }
        public int? MemberStatusId { get; set; }
        public string MemberStatusText { get; set; } = string.Empty;
        public int? MemberActiveStatusId { get; set; }
        public string MemberActiveStatusText { get; set; } = string.Empty;         
        public int MemberTypeId { get; set; }
        public string MemberTypeText { get; set; } = string.Empty;
        public string? Spouse { get; set; }
        public string? SpouseOccupation { get; set; }
        public bool IsActive { get; set; }
        public bool HasSubscription { get; set; }
        public int? MemberProfessionId { get; set; }
        public string? MemberProfessionText { get; set; }
    }

    public class MemberLoginVm:Result
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<int> Roles { get; set; }= new List<int>();
        public bool IsVerified { get; set; }
        public string JWToken { get; set; } = string.Empty;
        public DateTime IssuedOn { get; set; }
        public DateTime ExpiresOn { get; set; }
        public string RefreshToken { get; set; } = string.Empty;

        public MemberLoginRes Data { get; set; } = new MemberLoginRes();
    }

    public class MemberLoginListVm : Result
    {
        public int DataCount { get; set; }
        public List<MemberLoginRes> DataList { get; set; } = new List<MemberLoginRes>();
    }


    public class ForgetPasswordReq
    {
        public string? PhoneNo { get; set; }
        public string? Email { get; set; }
        public int MemberId { get; set; }
        public string? CardNo { get; set; }
        public string? MemberShipNo { get; set; }
    }
    public class ForgetPasswordRes
    {
        public int MemberId { get; set; }
        public string? Otp { get; set; }
        public DateTime? OtpCreatedTime { get; set; }

    }

    public class NewPasswordReq
    {
        public int MemberId { get; set; }
        public string? NewPassword { get; set; }   
        public string? ConfirmPassword { get; set; }
        public string? Otp { get; set; }

    }
    #endregion

}
