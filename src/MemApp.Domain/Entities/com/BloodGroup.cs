using MemApp.Domain.Core.Models;
using MemApp.Domain.Entities.mem;

namespace MemApp.Domain.Entities.com
{
    public class BloodGroup : BaseEntity
    {
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; }=string.Empty;
        public ICollection<RegisterMember>? RegisterMembers { get; set; }
        public ICollection<MemberTemp>? MemberTemps { get; set; }

    }
}
