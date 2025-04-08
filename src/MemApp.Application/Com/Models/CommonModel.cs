using MemApp.Application.Extensions;

namespace MemApp.Application.Com.Models
{
    public class CommonModel
    {
    }

    #region blood group
    public class BloodGroupReq
    {
        public string Code { get; set; }=string.Empty;
        public string? Title { get; set; }

        public int Id { get; set; }
    }
    public class BloodGroupRes
    {
        public string Code { get; set; } = string.Empty;
        public string? Title { get; set; }
        public int Id { get; set; }
    }
    public class BloodGroupVm : Result
    {
        public BloodGroupRes Data = new BloodGroupRes();
    }

    public class BloodGroupListVm : Result
    {
        public int DataCount { get; set; }
        public List<BloodGroupRes> DataList { get; set; } = new List<BloodGroupRes>();
    }
    #endregion
    #region smslog
    public class SmsLogReq
    {
        public string PhoneNo { get; set; }=string.Empty;
        public DateTime SmsDate { get; set; }
        public string? MemberName { get; set; }
        public string? MemberShipNo { get; set; }
        public string? Message { get; set; }
        public string Status { get; set; } = string.Empty;
        public int Id { get; set; }
    }
    public class SmsLogSearchReq
    {
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string? SearchText { get; set; }
    }
  
    public class SmsLogVm : Result
    {
        public SmsLogReq Data { get; set; } = new SmsLogReq();
    }

    public class SmsLogListVm : Result
    {
        public int DataCount { get; set; }
        public List<SmsLogReq> DataList { get; set; } = new List<SmsLogReq>();
    }
    #endregion
    #region release version
    public class ReleaseVersionReq
    {
        public string ReleaseTitle { get; set; } = string.Empty;
        public string ReleaseType { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public bool IsRequired { get; set; }
        public string AppId { get; set; } = string.Empty;

        public int Id { get; set; }
    }
   
    public class ReleaseVersionVm : Result
    {
        public ReleaseVersionReq Data { get; set; } = new ReleaseVersionReq();
    }

    public class ReleaseVersionListVm : Result
    {
        public int DataCount { get; set; }
        public List<ReleaseVersionReq> DataList { get; set; } = new List<ReleaseVersionReq>();
    }
    #endregion

    #region payment Method
    public class PaymentMethodRes
    {
        public string Code { get; set; } = string.Empty;
        public string? Title { get; set; }
        public int Id { get; set; }
    }
    public class PaymentMethodVm : Result
    {
        public PaymentMethodRes Data = new PaymentMethodRes();
    }

    public class PaymentMethodListVm : Result
    {
        public int DataCount { get; set; }
        public List<PaymentMethodRes> DataList { get; set; } = new List<PaymentMethodRes>();
    }
    #endregion

    #region Bank
    public class BankReq
    {
        public int slno { get; set; }
        public string? BANKNAME { get; set; }
    }
    public class BankListVm : Result
    {
        public int DataCount { get; set; }
        public List<BankReq> DataList { get; set; } = new List<BankReq>();
    }
    #endregion

    #region CreditCard
    public class CreditCardReq
    {
        public int slno { get; set; }
        public string? CardName { get; set; }
        public decimal? BankCommission { get; set; }

    }
    public class CreditCardListVm : Result
    {
        public int DataCount { get; set; }
        public List<CreditCardReq> DataList { get; set; } = new List<CreditCardReq>();
    }
    #endregion
}
