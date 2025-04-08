using MemApp.Domain.Core.Models;

namespace MemApp.Domain.Entities
{
    public class UserConference : BaseEntity
    {
        public string UserName { get; set; }
        public string UserId { get; set; }
        public DateTime LogInDate { get; set; }
        public DateTime? LogOutDate { get; set; }
        public bool LogOutStatus { get; set; }
        public string? AppId { get; set; }
        public string? IpAddress { get; set; }
        public string? UserToken { get; set; }
        public string? UserRefToken { get; set; }
    }
}
