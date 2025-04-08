using MemApp.Domain.Core.Models;

namespace MemApp.Domain.Entities.mem
{
    public class MemberFeesMap : BaseEntity
    {
        public int RegisterMemberId { get; set; }
        public RegisterMember RegisterMember { get; set; }
        public int MemberShipFeeId { get; set; }
        public MemberShipFee MemberShipFee { get; set; }
        public string MemberFeesTitle { get; set; }
        public decimal Amount { get; set; }
        public bool IsActive { get; set; }
        public DateTime PaymentDate { get; set; }

    }
}
