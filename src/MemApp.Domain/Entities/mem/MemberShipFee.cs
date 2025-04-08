using MemApp.Domain.Core.Models;

namespace MemApp.Domain.Entities.mem
{
    public class MemberShipFee:BaseEntity
    {
        public string Title { get; set; }
        public double Amount { get; set; }
        public string? DisplayName { get; set; }
        public bool IsActive { get; set; }
        public bool IsCurrent { get; set; }
        public int MemberTypeId { get; set; }
        public MemberType MemberType { get; set; }  
        public ICollection<MemberFeesMap> MemberFeesMaps { get; set; }
    }
}
