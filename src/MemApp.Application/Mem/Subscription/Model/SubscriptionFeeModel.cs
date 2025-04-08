using MemApp.Application.Extensions;
using MemApp.Application.Mem.TopUps.Models;
using MemApp.Application.Models;
using System.Text.Json.Serialization;

namespace MemApp.Application.Mem.Subscription.Model
{
    public class SubscriptionFeeModel
    {

        public int MemberId { get; set; }
        public List<MemberSubFee> Model { get; set; } = new List<MemberSubFee>();
        public TopUpReq? topup { get; set; }
    }

    #region SubscriptionFeeModel

    #region ServiceType
    public class SubscriptionFeeReq
    {
        public int Id { get; set; }
        public string SubscribedYear { get; set; } = string.Empty;
        public string? SubscribedEndYear { get; set; }

        public List<string> SubscribedQuaters { get; set; }= new List<string>();
        public  List<int> SubscriptionModIds { get; set; }= new List<int>();
        public string SubscribedQuater { get; set; } = string.Empty;
        public int SubscriptionModId { get; set; }
        public Decimal SubscriptionFee { get; set; }
        public Decimal? LateFee { get; set; }
        public Decimal? AbroadFee { get; set; }
        public string? CommandType { get; set; }

    }
    public class SubscriptionFeeDto
    {
        public int Id { get; set; }
        public string SubscribedYear { get; set; } = string.Empty;
        public int SubscriptionModId { get; set; }
        public decimal? PaymentFees { get; set; }
        public DateTime? PaymentFeesDate { get; set; }
        public Decimal SubscriptionFee { get; set; }
        public Decimal? LateFee { get; set; }
        public Decimal? AbroadFee { get; set; }
        public string? SubscriptionModName { get; set; }
        public int SubscriptionModeOrderBy { get; set; }
    }
    public class SubscriptionFeeDetailReq
    {
        public string Title { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    
    


    public class SubscriptionMember
    {
        public int MemberId { get; set; }
        public string MemberShipNo { get; set; } = string.Empty;
        public DateTime? PaidTill { get; set; }
        public string? MemberActiveStatusText { get; set; }

    }

    public class MemberSubFee
    {
        public int Id { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentNo { get; set; }  = string.Empty;
        public string MemberShipNo { get; set; } = string.Empty;
        public decimal TotalPaymentAmount { get; set; }
        public string? MemberName { get; set; }

        public DateTime? ActualPaymentDate { get; set; }
        public string PaymentYear { get; set; } = string.Empty;
        public string? SubscriptionName { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal LateAmount { get; set; }
        public decimal LateFePer { get; set; }   
        public bool IsChecked { get; set; }
        public decimal AbroadFeeAmount { get; set; }
        public decimal AbroadFeePer { get; set; }   
    }


    public class MemberSubFeeListVm : Result
    {
        public int DataCount { get; set; }
        public List<MemberSubFee> DataList { get; set; } = new List<MemberSubFee>();
    }

    public class ExportSubscriptionDue
    {
        public string MemberShipNo { get; set; } = string.Empty;
        public string DueAmount { get; set; } = string.Empty;   
        public string Name { get; set; } = string.Empty;
        public double LateAmount { get; set; }
        public string SubscriptionYear { get; set; } = string.Empty;
        public double LateFee { get; set; }
        public DateTime PaidTill { get; set; }
        public bool IsDue { get; set; }
        public double SubscriptionFee { get; set; }
    }
    public class ExportSubscriptionPayment
    {
        public string Name { get; set; } = string.Empty;
        public string MemberShipNo { get; set; } = string.Empty;

        public double PaymentAmount { get; set; }
        public double SubscriptionFee { get; set; }
        
        public string SubscriptionYear { get; set; } = string.Empty;
        public string SubscriptionName { get; set; } = string.Empty;
        public double LateAmount { get; set; }
        public double LateFee { get; set; }
        public DateTime PaidTill { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime ActualPaymentDate { get; set; }
        public bool IsDue { get; set; }
    }
    #endregion

    #endregion
}
