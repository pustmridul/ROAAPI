using MemApp.Domain.Core.Models;
using MemApp.Domain.Entities.Payment;
using MemApp.Domain.Entities.subscription;
using Res.Domain.Entities.ROAPayment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Res.Domain.Entities.ROASubscription
{
    public class ROASubscriptionFee : BaseEntity
    {
        public int SubscribedYear { get; set; }
        public DateTime? SubscribedMonth { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

      //  public int SubscriptionModId { get; set; }
        public Decimal SubscriptionFee { get; set; }
        public Decimal? LateFee { get; set; }
      //  public Decimal? AbroadFee { get; set; }
        public bool IsActive { get; set; }
       // public int MonthLength { get; set; }
      //  public SubscriptionMode SubscriptionMode { get; set; }
        //public ICollection<ROASubscriptionPaymentDetail>? SubscriptionPaymentDetails { get; set; }
    }
}
