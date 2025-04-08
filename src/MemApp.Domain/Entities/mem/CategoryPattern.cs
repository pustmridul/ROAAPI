using MemApp.Domain.Core.Models;

namespace MemApp.Domain.Entities.mem
{
    public class CategoryPattern:BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public ICollection<MemberType> MemberTypes { get; set; }
    }
}
