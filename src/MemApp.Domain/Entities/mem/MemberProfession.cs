using MemApp.Domain.Core.Models;

namespace MemApp.Domain.Entities.mem
{
    public class MemberProfession : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public ICollection<RegisterMember> RegisterMembers { get; set; }
        public ICollection<MemberTemp> MemberTemps { get; set; }
    }
}
