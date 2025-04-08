using MemApp.Domain.Core.Models;

namespace MemApp.Domain.Entities.mem
{
    public class MemberType : BaseEntity
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsSubscribed { get; set; } = false;
        public string? OldId { get; set; }
        public ICollection<RegisterMember> RegisterMembers { get; set; }
        public CategoryPattern CategoryPatterns { get; set; }
        public int CategoryPatternId { get; set; }
        public ICollection<MemberShipFee> MemberShipFees { get; set; }

    }
}
