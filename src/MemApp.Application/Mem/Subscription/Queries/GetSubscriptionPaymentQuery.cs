using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Subscription.Model;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Subscription.Queries
{

    public class GetSubscriptionPaymentQuery : IRequest<ListResult<MemberSubFee>>
    {
        public string PaymentNo { get; set; } = string.Empty;

    }

    public class GetSubscriptionPaymentQueryHandler : IRequestHandler<GetSubscriptionPaymentQuery, ListResult<MemberSubFee>>
    {
        private readonly IMemDbContext _context;
        public GetSubscriptionPaymentQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<ListResult<MemberSubFee>> Handle(GetSubscriptionPaymentQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = new ListResult<MemberSubFee>();

                var data = await _context.SubscriptionPaymentTemps
                    .Include(i => i.RegisterMember)
                    .Where(q => q.PaymentNo == request.PaymentNo)
                    .OrderByDescending(q => q.PaymentDate)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);


                result.HasError = false;
                result.Data = data.Select(s => new MemberSubFee
                {
                    Id = s.Id,
                    PaymentDate = s.PaymentDate,
                    PaymentNo = s.PaymentNo,
                    MemberShipNo = s.MemberShipNo,
                    MemberName = s.RegisterMember?.FullName,
                    TotalPaymentAmount = data.Sum(s1 => s1.PaymentAmount) + data.Sum(s2 => s2.LateAmount),
                    ActualPaymentDate = s.ActualPaymentDate,

                    PaymentYear = s.SubscriptionYear ?? "",
                    SubscriptionName = s.SubscriptionName,
                    PaymentAmount = s.PaymentAmount,
                    LateAmount = s.LateAmount,
                    LateFePer = s.LateFeePer,
                    IsChecked = s.IsPaid,
                    AbroadFeeAmount = s.AbroadFeeAmount,
                    AbroadFeePer = s.AbroadFeePer,


                }).ToList();
                return result;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
