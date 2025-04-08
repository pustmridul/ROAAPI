using MemApp.Domain.Core.Models;
using MemApp.Domain.Entities.com;
using MemApp.Domain.Entities.mem;
using MemApp.Domain.Entities.Payment;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Res.Domain.Entities.ROAPayment
{
    public class ROASubscriptionPayment : BaseEntity
    {
       
            public int MemberId { get; set; }
            public string MemberShipNo { get; set; } = string.Empty;
            public string PaymentNo { get; set; } = string.Empty;
            public string? PaymentType { get; set; } = string.Empty;
            public MemberRegistrationInfo Member { get; set; }

            //public int TopUpId {  get; set; }

            //public TopUp TopUp { get; set; }
            
         
            public DateTime? ActualPaymentDate { get; set; }
            public bool IsPaid { get; set; }

            public string? SubsStatus { get; set; }
            public decimal? TotalAmount { get; set; }

            //[ForeignKey("PaymentMethod")]
            //public int? PaymentTypeID { get; set; }
            //public PaymentMethod? PaymentMethod { get; set; }

            //[ForeignKey("Bank")]
            //public decimal? BankSlno { get; set; }
            //public Bank? Bank { get; set; }

            //[ForeignKey("CreditCard")]
            //public decimal? CardSlno { get; set; }
            
            //public CreditCard? CreditCard { get; set; }
            public ICollection<ROASubscriptionPaymentDetail>? SubscriptionPaymentDetails { get; set; }
        
    }
}
