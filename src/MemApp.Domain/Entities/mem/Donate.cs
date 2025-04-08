using MemApp.Domain.Core.Models;

namespace MemApp.Domain.Entities.mem;

public class Donate : BaseEntity
{
    public int? MemberId { get; set; }
    public int DonationId { get; set; }
    public string DonateNo { get; set; }
    public decimal Amount { get; set; }
    public string? Note { get; set; }
    public DateTime DonateDate { get; set; }

    public Donation Donation { get; set; }
    public RegisterMember? RegisterMember { get; set; }
}
