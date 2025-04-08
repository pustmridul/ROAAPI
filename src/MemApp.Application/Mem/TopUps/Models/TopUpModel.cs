using MemApp.Application.Extensions;
using MemApp.Application.Models;

namespace MemApp.Application.Mem.TopUps.Models
{
    public class TopUpModel
    {

    }
    public class TopUpReqest
    {
        public string TransId { get; set; } = string.Empty;
        public int MemberId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string? TransStatus { get; set; }
    }
    public class TopUpReqestVm : Result
    {
        public TopUpReqest Data { get; set; }= new TopUpReqest();
    }
    public class TopUpReq
    {
        public int Id { get; set; }
        public int RegisterMemberId { get; set; }
        public int MemberId { get; set; }
        public DateTime TopUpDate { get; set; }
        public string? MemberShipNo { get; set; }
        public string? MemberCardNo { get; set; }
        public decimal CurrentBalance { get; set; }
        public string? MemberName { get; set; }
        public string? CreateByName { get; set; }
        public string? CardNo { get; set; }
        public decimal TotalAmount { get; set; }
        public bool OnlineTopUp { get; set; }
        public string? TransId { get; set; }
        public string? Currency { get; set; }
        public bool OfflineTopUp { get; set; }
        public string? PaymentMode { get; set; }   
        public string? Note { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? CreatedBy { get; set; }
        public List<TopUpDetailReq> TopUpDetails { get; set; }= new List<TopUpDetailReq>();
    }
    public class TopUpDetailReq
    {
        public string? TOPUPNO { get; set; }
        public int Id { get; set; }
        public int TopUpId { get; set; }
        public string? PaymentMethodText { get; set; }
        public int PaymentMethodId { get; set; }
        public int? BankId { get; set; }
        public string? BankText { get; set; }
        public int? CreditCardId { get; set; }
        public string? CreditCardText { get; set; }

        public string? TrxNo { get; set; }
        public string? MachineNo { get; set; }
        public string? TrxCardNo { get; set; }
        public decimal Amount { get; set; }

    }
    public class TopUpVm : Result
    {
        public TopUpReq Data { get; set; } = new TopUpReq();
    }
    public class TopUpListVm : Result
    {
        public long DataCount { get; set; }
        public List<TopUpReq> DataList { get; set; } = new List<TopUpReq>();
    }

    public class VerifyTopUpVm : Result
    {

    }

    public class CurrentBalabnceReq
    {
        public decimal CurrentBalabnce { get; set; }
    }

    public class TopUpExportReq
    {
        public string MemberShipNo { get; set; } = string.Empty;
        public string MemberName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public string TopUpId { get; set; } = string.Empty;
        public string TopUpDate { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PayType { get; set; } = string.Empty;
        public string TrxNo { get; set; } = string.Empty;
        public string BankText { get; set; } = string.Empty;
        public string TrxCardNo { get; set; } = string.Empty;
        public string MachineNo { get; set; } = string.Empty;
        public string Dates { get; set; } = string.Empty;
        public string? UserName { get; set; }

        


    }
}
