using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Subscription.Model;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Subscription.Queries
{

    public class GetSubscriptionPaymentListQuery : IRequest<ListResult<MemberSubFee>>
    {
        public string? MemberShipNo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }

    public class GetSubscriptionPaymentListQueryHandler : IRequestHandler<GetSubscriptionPaymentListQuery, ListResult<MemberSubFee>>
    {
        private readonly IMemDbContext _context;
        public GetSubscriptionPaymentListQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<ListResult<MemberSubFee>> Handle(GetSubscriptionPaymentListQuery request, CancellationToken cancellationToken)
        {
            try
            {
                DateTime st = request.StartDate.AddDays(-1);
                DateTime ed = request.EndDate.AddDays(1);
                var result = new ListResult<MemberSubFee>();


                var data = await _context.SubscriptionPaymentTemps
                    .Include(i => i.RegisterMember)
                    .Where(q => q.ActualPaymentDate >= st && q.ActualPaymentDate <= ed
                    && (request.MemberShipNo != null ? q.MemberShipNo == request.MemberShipNo : true))
                    .GroupBy(g => new
                    {
                        g.MemberShipNo,
                        g.PaymentNo
                    })
                    .Select(s => new
                    {
                        PaymentNo = s.Key.PaymentNo,
                        MemberShipNo = s.Key.MemberShipNo,
                        FirstMember = s.FirstOrDefault(),
                        TotalPaymentAmount = s.Sum(s1 => s1.PaymentAmount) + s.Sum(s1 => s1.LateAmount),
                        ActualPaymentDate = s.Min(x => x.ActualPaymentDate)
                    })
                .OrderByDescending(o => o.ActualPaymentDate)
                .AsNoTracking()
                .ToPaginatedListAsync(request.PageNo, request.PageSize, cancellationToken);

                var datalist = data.Data
                    .Select(s => new MemberSubFee
                    {
                        PaymentNo = s.PaymentNo,
                        MemberShipNo = s.MemberShipNo,
                        MemberName = s.FirstMember?.RegisterMember?.FullName ?? "",
                        TotalPaymentAmount = s.TotalPaymentAmount,
                        ActualPaymentDate = s.ActualPaymentDate

                    }).ToList();
                result.Count = Convert.ToInt32(data.TotalCount);
                result.HasError = false;
                result.Data = datalist;
                return result;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
