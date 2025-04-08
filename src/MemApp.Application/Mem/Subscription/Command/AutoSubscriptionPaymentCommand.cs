using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Subscription.Model;
using MemApp.Application.Models;
using MemApp.Domain.Entities.Payment;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Subscription.Command
{
    public class AutoSubscriptionPaymentCommand : IRequest<Result>
    {
        public string SubscriptionYear { get; set; }
        public string QuaterNo { get; set; }
    }

    public class AutoSubscriptionPaymentCommandHandler : IRequestHandler<AutoSubscriptionPaymentCommand, Result>
    {
        private readonly IMemDbContext _context;
        public AutoSubscriptionPaymentCommandHandler(IMemDbContext context)
        {
            _context = context;
        }
        public async Task<Result> Handle(AutoSubscriptionPaymentCommand request, CancellationToken cancellation)
        {
            var result = new Result();

            var memberList= await _context.RegisterMembers
                .Include(i => i.MemberTypes)
                .Include(i=>i.MemberActiveStatus)
                .Where(q => q.MemberTypes.IsSubscribed 
                && q.IsActive 
                && q.IsMasterMember==true
                && q.PaidTill < DateTime.Now
                )
                .Select(s =>
            new SubscriptionMember
            {
              PaidTill= s.PaidTill,
              MemberShipNo=  s.MembershipNo,
              MemberId=  s.Id,
                MemberActiveStatusText = s.MemberActiveStatus.Name
            }).ToListAsync(cancellation);


            var subsDueFeeList = await _context.SubscriptionFees
                .Where(
                q=>q.SubscribedYear== request.SubscriptionYear
                && q.Title == request.QuaterNo
                )
                .ToListAsync(cancellation);
            List< SubscriptionPaymentTemp> subscriptionPaymentTempList = new List< SubscriptionPaymentTemp >();
            foreach( var sub in subsDueFeeList )
            {
                foreach (var m in memberList)
                {
                    var subPayment = await _context.SubscriptionPaymentTemps.SingleOrDefaultAsync(q =>
                    q.RegisterMemberId == m.MemberId
                    && q.SubscriptionFeesId ==sub.Id,
                    cancellation);

                    if(subPayment == null)
                    {
                        subPayment = new SubscriptionPaymentTemp()
                        {
                            RegisterMemberId= m.MemberId,
                            SubscriptionFeesId=sub.Id,
                            SubscriptionName= sub.Title,
                            SubsStatus ="Due",
                            LateFeePer= sub.LateFee??0,
                            PaymentAmount= m.MemberActiveStatusText!= "Abroad" ? sub.SubscriptionFee: (sub.AbroadFee ?? 0 / 100) * sub.SubscriptionFee,
                            PaymentDate= sub.EndDate,
                            IsPaid=false,
                            MemberShipNo=m.MemberShipNo,
                            AbroadFeePer= sub.AbroadFee ??0,
                            AbroadFeeAmount=( sub.AbroadFee??0 /100)*sub.SubscriptionFee
                        };
                        subscriptionPaymentTempList.Add(subPayment);
                    }
                }
            }
           _context.SubscriptionPaymentTemps.AddRange(subscriptionPaymentTempList);

            await _context.SaveChangesAsync(cancellation);


            return result;
        }

    }
}
