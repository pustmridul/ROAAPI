using MemApp.Domain.Core.Models;

namespace MemApp.Domain.Entities.mem
{
    public class MemberChildren : BaseEntity
    {
        public string CadetNo { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string ContactName { get; set; } = string.Empty;
        public DateTime? Dob { get; set; }
        public string Gender { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public RegisterMember RegisterMembers { get; set; }
        public int RegisterMemberId { get; set; }
        public MemberTemp MemberTemp { get; set; }
    }
}
