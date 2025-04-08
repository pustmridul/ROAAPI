using AutoMapper.Configuration.Annotations;
using MemApp.Application.Mem.Subscription.Model;
using MemApp.Application.Mem.TopUps.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResApp.Application.Com.Commands.ROAPayment.Models
{
    public class SubscriptionSubDuePayReq
    {
        public int MemberId { get; set; }
        //public string PaymentType { get; set; }
        //public int? PaymentTypeId { get; set; }
        //public int? BankId { get; set; }
        //public int? CardId { get; set; }
        //public decimal? Amount { get; set; }
        public List<MemberSubPaymentReqDetails> Model { get; set; } = new List<MemberSubPaymentReqDetails>(); 
        public TopUpReq? topup { get; set; }
    }

    public class MemberSubPaymentReqDetails
    {
        [Ignore]
        public int RoSubscriptionDueId {  get; set; }
        public int Id { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentNo { get; set; } = string.Empty;

        [Ignore]
        public string MemberShipNo { get; set; } = string.Empty;
        public decimal TotalPaymentAmount { get; set; }
        public string? MemberName { get; set; }

        public DateTime? ActualPaymentDate { get; set; }
        [Ignore]
        public string PaymentYear { get; set; } = string.Empty;
        public string? SubscriptionName { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal LateAmount { get; set; }
      //  public decimal LateFePer { get; set; }
        public bool IsChecked { get; set; }

        public DateTime SubscriptionMonth {  get; set; }
        public int SubscriptionYear {  get; set; }
       
    }

    public class MemberPaymentReqSSl
    {
        public int Id { get; set; }
        public string? SubscriptionName { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal LateAmount { get; set; }
       
        public bool IsChecked { get; set; }

        public DateTime SubscriptionMonth { get; set; }
        public int SubscriptionYear { get; set; }
    }

    public class PaymentTracking
    {
        public DateTime SubscriptionMonth { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal LateAmount { get; set; }
    }

    public class MemberSubPaymentRes : MemberSubPaymentReqDetails
    {
        public int MemberId {  get; set; }
        public string MemberShipNo {  get; set; }
        public string SubscriptionDetails {  get; set; }
    }

    public class PaymentReport
    {
        public string MemberShipNo { get; set; }
        public string MemberName { get; set; }
        public string SubscriptionDetails { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal TotalPaymentAmount { get; set; }
        public string PaymentNo { get; set; }

        public List<MemberSubPaymentRes> PaymentDetails { get; set; }

        
    }
}
