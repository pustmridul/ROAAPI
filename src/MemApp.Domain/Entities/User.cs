using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using MemApp.Domain.Core.Models;
using MemApp.Domain.Entities;
using MemApp.Domain.Enums;

namespace Res.Domain.Entities
{

    public class User : BaseEntity
    {
        public string? Name { get; set; }
        public string? UserName { get; set; }
        public string? EmailId { get; set; }
        public bool? EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public int LoginFailedAttemptCount { get; set; }
        public DateTime? LastLoginFailedAttemptDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string? PhoneNo { get; set; }
        public bool IsActive { get; set; }
        public int? LastPasswordResetBy { get; set; }
        public string? LastPasswordResetByName { get; set; }
        public DateTime? LastPasswordResetOn { get; set; }
        public string AppId { get; set; }
        public int? MemberId { get; set; }
        public int? MemberInfoId { get; set; }

        [ForeignKey(nameof(MemberInfoId))]
        public virtual MemberRegistrationInfo? Member { get; set; }

        public string? Otp { get; set; }
        public DateTime? OtpCreatedTime { get; set; }

        //    public string? CompanyName {get; set;}
        //   public string? TradeLicense {get; set;}
        //    public string? UserNID {get; set;}


        //  public 
        [JsonIgnore]
        public virtual ICollection<UserRoleMap> UserRoles { get; set; }
        [JsonIgnore]
        public virtual ICollection<UserMenuMap> UserMenuMaps { get; set; }

        //  public virtual ICollection<MemberRegistrationInfo>? MemberRegistrationInfos { get; set; }
        public int ChangePinCount { get; set; }
    }
}
