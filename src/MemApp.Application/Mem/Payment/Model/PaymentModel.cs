using MemApp.Application.Extensions;
using MemApp.Application.Models;

namespace MemApp.Application.Mem.Payment.Model
{
    public class PaymentModel
    {
    }

    #region SubscriptionPay

    public class SubscriptionPayReq
    {
        public int Id { get; set; }
        public int RegisterMemberId { get; set; }
        public Decimal MemberPayment { get; set; }
        public DateTime PaymentDate { get; set; }
    }
    public class SubscriptionPayRes
    {
        public int Id { get; set; }
        public int RegisterMemberId { get; set; }
        public Decimal MemberPayment { get; set; }
        public DateTime PaymentDate { get; set; }
    }
    public class SubscriptionPayVm : Result
    {
        public SubscriptionPayRes Data = new SubscriptionPayRes();
    }

    public class SubscriptionPayListVm : Result
    {
        public int DataCount { get; set; }
        public List<SubscriptionPayRes> DataList { get; set; } = new List<SubscriptionPayRes>();
    }
    #endregion

    #region SubscriptionPayDetail
    public class SubscriptionPaymentReq
    {
        public int RegisterMemberId { get; set; }
        public string MemberShipNo { get; set; }
        public Decimal PaymentAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public List<SubscriptionPayDetailReq> SubscriptionPayDetails { get; set; } = new List<SubscriptionPayDetailReq>();

    }
    
    public class SubscriptionPayDetailReq
    {
        public int SubscriptionFeesId { get; set; }
        public Decimal PaymentFees { get; set;}
    }

    public class SubscriptionPayDetailRes
    {
        public int Id { get; set; }
        public Decimal CustomerDue { get; set; }
        public int SubscriptionFeesId { get; set; }
        public int SubscriptionPaymentId { get; set; }
    }
    public class SubscriptionPayDetailVm : Result
    {
        public SubscriptionPayDetailRes Data = new SubscriptionPayDetailRes();
    }

    public class SubscriptionPayDetailListVm : Result
    {
        public int DataCount { get; set; }
        public List<SubscriptionPayDetailRes> DataList { get; set; } = new List<SubscriptionPayDetailRes>();
    }

    #endregion


    #region TransactionModel
    public class TransactionReq
    {
        public int Id { get; set; }
        public int Tran_id { get; set; }
    }
    public class TransactionRes
    {
        public int Id { get; set; }
        public int Tran_id { get; set; }
    }
    public class TransactionVm : Result
    {
        public TransactionRes Data { get; set; } = new TransactionRes();
    }
    public class TransactionVmList : Result
    {
        public int DataCount { get; set; }
        public List<TransactionRes> DataList { get; set; } = new List<TransactionRes>();
    }
    #endregion
}
