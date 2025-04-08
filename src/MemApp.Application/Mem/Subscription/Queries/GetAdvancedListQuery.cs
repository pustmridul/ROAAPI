using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Subscription.Model;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Subscription.Queries
{

    public class GetAdvancedListQuery : IRequest<MemberSubFeeListVm>
    {
        public int Id { get; set; }
    }

    public class GetAdvancedListQueryHandler : IRequestHandler<GetAdvancedListQuery, MemberSubFeeListVm>
    {
        private readonly IMemDbContext _context;
        public GetAdvancedListQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<MemberSubFeeListVm> Handle(GetAdvancedListQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = new MemberSubFeeListVm();
                var dateTostart = DateTime.Now;

                var member = await _context.RegisterMembers
                    .Include(i => i.MemberTypes)
                    .Include(i => i.MemberActiveStatus)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(q => q.Id == request.Id, cancellationToken);

                if (member == null)
                {
                    result.HasError = false;
                    result.Messages?.Add("Member Not Found");
                    return result;
                }
                else
                {
                    var memberType = await _context.MemberTypes
                        .AsNoTracking()
                        .SingleOrDefaultAsync(q => q.Id == member.MemberTypeId, cancellationToken);
                    if (memberType != null)
                    {
                        if (!memberType.IsSubscribed)
                        {
                            result.HasError = false;
                            result.Messages?.Add("This membership type no needed subscription.");
                            return result;
                        }
                    }
                   

                    if (member.PaidTill == null || member.PaidTill < DateTime.Now.Date)
                    {
                        dateTostart = DateTime.Now.Date;
                    }
                    else
                    {
                        dateTostart = member.PaidTill ?? dateTostart;
                    }

                    var dataList = await _context.SubscriptionFees
                        .Where(q => q.IsActive && q.StartDate.Date > dateTostart)
                        .OrderBy(o => o.SubscribedYear)
                        .ThenBy(o => o.StartDate)
                        .AsNoTracking()
                        .ToListAsync(cancellationToken);

                    if (dataList.Count == 0)
                    {
                        result.HasError = false;
                        result.Messages?.Add("Data not found");
                    }
                    result.DataList = dataList.Select(x => new MemberSubFee
                    {
                        Id = x.Id,
                        SubscriptionName = x.Title,
                        AbroadFeePer = x.AbroadFee ?? 0,
                        AbroadFeeAmount = (member.MemberActiveStatus?.Name == "Abroad" || member.MemberActiveStatus?.Name == "Absentee") ? ((x.AbroadFee ?? 0) / 100) * x.SubscriptionFee :
                        x.SubscriptionFee,
                        PaymentDate = x.StartDate,
                        PaymentYear = x.SubscribedYear,
                        PaymentAmount = (member.MemberActiveStatus?.Name == "Abroad" || member.MemberActiveStatus?.Name == "Absentee") ? ((x.AbroadFee ?? 0) / 100) * x.SubscriptionFee :
                        x.SubscriptionFee,
                        ActualPaymentDate = DateTime.Now,

                    }).ToList();
                    return result;
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.ToString());
            }  
        }
    }
}
