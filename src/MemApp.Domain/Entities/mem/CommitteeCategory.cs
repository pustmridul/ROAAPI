using MemApp.Domain.Core.Models;

namespace MemApp.Domain.Entities.mem
{
    public class CommitteeCategory : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public ICollection<Committee>? Committees { get; set; }
    }
}
