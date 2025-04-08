using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Subscription.Model;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace MemApp.Application.Mem.Subscription.Queries
{

    public class GetPaidListByMemberIdQuery : IRequest<MemberSubFeeListVm>
    {
        public int Id { get; set; }
    }

    public class GetPaidListByMemberIdQueryHandler : IRequestHandler<GetPaidListByMemberIdQuery, MemberSubFeeListVm>
    {
        private readonly IMemDbContext _context;
        public GetPaidListByMemberIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<MemberSubFeeListVm> Handle(GetPaidListByMemberIdQuery request, CancellationToken cancellationToken)
        {
            var result = new MemberSubFeeListVm();

            var dataList = await _context.SubscriptionPaymentTemps.Where(q=>
                q.IsPaid== true
                && q.RegisterMemberId== request.Id
                ).OrderBy(o=> o.SubscriptionYear).ThenBy(o=>o.PaymentDate)
                .AsNoTracking()
                .ToListAsync();

                if (dataList.Count > 0)
                {
                    var memberObj = await _context.RegisterMembers.Select(s=> new {s.Id, s.MemberTypeId})
                        .SingleOrDefaultAsync(q => q.Id == request.Id, cancellationToken);
                    if (memberObj != null)
                    {
                        var memberType = await _context.MemberTypes.SingleOrDefaultAsync(q => q.Id == memberObj.MemberTypeId, cancellationToken);
                        if (memberType != null)
                        {
                            if (!memberType.IsSubscribed)
                            {
                                result.HasError = false;
                                result.Messages?.Add("This membership type no needed subscription.");
                                return result;
                            }
                        }
                    }
               
                    result.HasError = false;
                        result.DataList = dataList.Select( x => new MemberSubFee
                        {
                            Id = x.Id,
                            PaymentDate = x.PaymentDate,
                            ActualPaymentDate= x.ActualPaymentDate,
                            PaymentYear= x.SubscriptionYear ??"",
                            PaymentAmount= x.PaymentAmount,
                            LateAmount=x.LateAmount,
                            SubscriptionName=x.SubscriptionName,

                        }).ToList();
                }
          
            return result;
        }
    }
}
