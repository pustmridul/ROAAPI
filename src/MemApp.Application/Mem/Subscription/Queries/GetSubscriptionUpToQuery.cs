using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Subscription.Model;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Subscription.Queries
{

    public class GetSubscriptionUpToQuery : IRequest<ListResult<SubscriptionFeeDto>>
    {
        public int Id { get; set; }
    }

    public class GetSubscriptionUpToQueryHandler : IRequestHandler<GetSubscriptionUpToQuery, ListResult<SubscriptionFeeDto>>
    {
        private readonly IMemDbContext _context;
        public GetSubscriptionUpToQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<ListResult<SubscriptionFeeDto>> Handle(GetSubscriptionUpToQuery request, CancellationToken cancellationToken)
        {
            var result = new ListResult<SubscriptionFeeDto>();
            //var data = await _context.RegisterMembers.Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            //if (data != null)
            //{
                var currentDateTime = DateTime.Now;
                int year = currentDateTime.Year;
                string yearString = year.ToString();

                var dataList = await _context.SubscriptionPaymentDetails.Where(x => x.RegisterMemberId == request.Id)
                    .Include(i=>i.SubscriptionFees).ThenInclude(i=>i.SubscriptionMode)  
                    .OrderByDescending(o=> o.PaymentFeesDate)
                    .ToListAsync(cancellationToken);
               
                if (dataList.Count > 0)
                {
                    result.HasError = false;
                    result.Data = dataList.Select(x => new SubscriptionFeeDto
                    {
                        Id = x.Id,
                        AbroadFee =x.SubscriptionFees?.AbroadFee,
                        LateFee = x.PaymentFees - x.SubscriptionFees?.SubscriptionFee,
                        SubscribedYear = x.SubscriptionFees.SubscribedYear,
                        SubscriptionFee = x.SubscriptionFees.SubscriptionFee,
                        SubscriptionModId = x.SubscriptionFees.SubscriptionModId,
                        SubscriptionModName = x.SubscriptionFees.SubscriptionMode.Name,
                        PaymentFees= x.PaymentFees,
                        PaymentFeesDate = x.PaymentFeesDate
                    }).ToList();
                }
            //}
            //else
            //{
            //    result.HasError = true;
          //      result.Messages.Add("Data Retrieve Failed");
            //}
            return result;
          
        }
    }
}
