using MemApp.Domain.Core.Models;
using MemApp.Domain.Entities.mem;
using Res.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemApp.Domain.Entities.com
{
    public class TopUp : BaseEntity
    {
      //  public int RegisterMemberId { get; set; }
     
      //  public RegisterMember RegisterMember { get; set; }

        [ForeignKey("MemberRegistrationInfo")]
        public int MemberId { get; set; }
        public MemberRegistrationInfo MemberRegistrationInfo { get; set; }
        public string MemberShipNo { get; set; } = string.Empty;
        public string? CardNo { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime TopUpDate { get; set; }
        public string? PaymentMode { get; set; }
        public string? Note { get; set; }
        public string Status { get; set; } = string.Empty;
        public int? VerifiedBy { get; set; }
        public string? VerifierName { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public bool IsActive { get; set; }
        public bool OfflineTopUp { get; set; }
        public bool OnlineTopUp { get; set; }

        public string? MonthDetails { get; set; }
        public string? PaymentFor { get; set; }


        public ICollection<TopUpDetail> TopUpDetails { get; set; }

    }
    public class TopUpDetail : BaseEntity
    {
        public int TopUpId { get; set; }
        public TopUp TopUp { get; set; }
        public string PaymentMethodText { get; set; } = string.Empty;
        public int PaymentMethodId { get; set; }
        public string? TrxNo { get; set; }
        public string? MachineNo { get; set; }
        public string? TrxCardNo { get; set; }
        public decimal Amount { get; set; }
        public bool IsActive { get; set; }
        public int? BankId { get; set; }
        public string? BankText { get; set; }
        public int? CreditCardId { get; set; }
        public string? CreditCardText { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }

}
