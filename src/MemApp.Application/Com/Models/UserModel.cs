using MemApp.Application.Extensions;
using MemApp.Domain.Enums;
using System.Text.Json.Serialization;

namespace MemApp.Application.Com.Models
{
    public class UserModel
    {
    }
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }= string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string EmailId { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }

        [JsonIgnore]
        public string PasswordHash { get; set; } = string.Empty;

        [JsonIgnore]
        public string PasswordSalt { get; set; }=string.Empty;

        [JsonIgnore]
        public string OldPassword { get; set; } = string.Empty;

        [JsonIgnore]
        public string NewPassword { get; set; } = string.Empty;

        [JsonIgnore]
        public string ConfirmPassword { get; set; } = string.Empty;

        [JsonIgnore]
        public int LoginFailedAttemptCount { get; set; }

        [JsonIgnore]
        public DateTime? LastLoginFailedAttemptDate { get; set; }

        [JsonIgnore]
        public DateTime? LastLoginDate { get; set; }

        [JsonIgnore]
        public UserStatus Status { get; set; }
        public string PhoneNo { get; set; } = string.Empty;

        [JsonIgnore]
        public bool IsActive { get; set; }
        public string AppId { get; set; } = string.Empty;

    }
    public class UserRes
    {
        public string Name { get; set; } = string.Empty;
        public string EmailId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? PhoneNo { get; set; }
        public int Id { get; set; }
       
    }
    public class UserVm : Result
    {
        public UserRes Data { get; set; } = new UserRes();
    }

    
    public class ChangePin :Result
    {
        public string OldPin { get; set; } = string.Empty;
        public string NewPin { get; set; }=string.Empty;
        public string ConfirmPin { get; set; } = string.Empty;

        public int MemberId { get; set; }

    }
    public class UserConferenceReq
    {
        public string UserName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public DateTime LogInDate { get; set; }
        public DateTime? LogOutDate { get; set; }
        public bool LogOutStatus { get; set; }
        public string? AppId { get; set; }
        public string? IpAddress { get; set; }
        public string? UserToken { get; set; }
        public string? UserRefToken { get; set; }
    }
    public class UserLogRes
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime LogDate { get; set; }
        public string LogText { get; set; } = string.Empty;
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
    }
}
