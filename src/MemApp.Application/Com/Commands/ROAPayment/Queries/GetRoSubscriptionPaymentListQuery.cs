using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Subscription.Model;
using Microsoft.EntityFrameworkCore;
using ResApp.Application.Com.Commands.ROAPayment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResApp.Application.Com.Commands.ROAPayment.Queries
{
    
    public class GetRoSubscriptionPaymentListQuery : IRequest<ListResult<MemberSubPaymentRes>>
    {
        public string? MemberShipNo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }

    public class GetRoSubscriptionPaymentListQueryHandler : IRequestHandler<GetRoSubscriptionPaymentListQuery, ListResult<MemberSubPaymentRes>>
    {
        private readonly IMemDbContext _context;
        public GetRoSubscriptionPaymentListQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<ListResult<MemberSubPaymentRes>> Handle(GetRoSubscriptionPaymentListQuery request, CancellationToken cancellationToken)
        {
            try
            {
                DateTime st = request.StartDate.AddDays(-1);
                DateTime ed = request.EndDate.AddDays(1);
                var result = new ListResult<MemberSubPaymentRes>();


                var data = await _context.ROASubscriptionPaymentDetail
                    .Include(i => i.Member)
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
                      //  TotalPaymentAmount = s.Sum(s1 => s1.PaymentAmount) + s.Sum(s1 => s1.LateAmount),
                        TotalPaymentAmount = s.Sum(s1 => s1.PaymentFees), //+ s.Sum(s1 => s1.LateAmount),
                        ActualPaymentDate = s.Min(x => x.ActualPaymentDate)
                    })
                .OrderByDescending(o => o.ActualPaymentDate)
                .AsNoTracking()
                .ToPaginatedListAsync(request.PageNo, request.PageSize, cancellationToken);

                var datalist = data.Data
                    .Select(s => new MemberSubPaymentRes
                    {
                        PaymentNo = s.PaymentNo,
                        MemberShipNo = s.MemberShipNo!,
                        MemberName = s.FirstMember?.Member?.Name ?? "",
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
