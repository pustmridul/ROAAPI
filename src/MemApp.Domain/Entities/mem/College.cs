using MemApp.Domain.Core.Models;

namespace MemApp.Domain.Entities.mem
{
    public class College : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public bool IsActive { get; set; }
        public string? OldId { get; set; }
        public ICollection<RegisterMember> RegisterMembers { get; set; }
        public ICollection<MemberTemp> MemberTemps { get; set; }

    }
}
