using MemApp.Domain.Core.Models;
using MemApp.Domain.Entities.com;
using MemApp.Domain.Entities.mem;
using MemApp.Domain.Entities.Payment;
using MemApp.Domain.Entities.subscription;
using Res.Domain.Entities.ROASubscription;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Res.Domain.Entities.ROAPayment
{
    public class ROASubscriptionPaymentDetail : BaseEntity
    {
       
        public string? MemberShipNo {  get; set; }
        public int? SubscriptionFeesId { get; set; }
        public DateTime PaymentFeesDate { get; set; }
        public DateTime ActualPaymentDate { get; set; }
        public Decimal PaymentFees { get; set; }
        public decimal LateFees { get; set; }
        public bool IsPaid { get; set; }

        public string SubscriptionName {  get; set; }
      //  public string SubsStatus {  get; set; }

        public int SubscriptionYear { get; set; }
        public DateTime SubscriptionMonth { get; set; }


        //  public ROASubscriptionFee? SubscriptionFee { get; set; }
        [ForeignKey("ROASubscriptionPayment")]
        public int SubscriptionPaymentId { get; set; }
        public ROASubscriptionPayment SubscriptionPayment { get; set; }

        public string PaymentNo {  get; set; }

        [ForeignKey("MemberRegistrationInfo")]
        public int? MemberId { get; set; }
        public MemberRegistrationInfo Member { get; set; }

        
    }
}
