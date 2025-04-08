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
    
    public class GetRoSubscriptionPaymentDetailsQuery : IRequest<ListResult<MemberSubPaymentRes>>
    {
        public string PaymentNo { get; set; } = string.Empty;

    }

    public class GetRoSubscriptionPaymentDetailsQueryHandler : IRequestHandler<GetRoSubscriptionPaymentDetailsQuery, ListResult<MemberSubPaymentRes>>
    {
        private readonly IMemDbContext _context;
        public GetRoSubscriptionPaymentDetailsQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<ListResult<MemberSubPaymentRes>> Handle(GetRoSubscriptionPaymentDetailsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = new ListResult<MemberSubPaymentRes>();

                var data = await _context.ROASubscriptionPaymentDetail
                    .Include(i => i.Member)
                    .Where(q => q.PaymentNo == request.PaymentNo)
                  //  .OrderByDescending(q => q.ActualPaymentDate)
                    .OrderBy(q => q.SubscriptionMonth)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);


                result.HasError = false;
                result.Data = data.Select(s => new MemberSubPaymentRes
                {
                    Id = s.Id,
                    PaymentDate = s.ActualPaymentDate,
                    PaymentNo = s.PaymentNo,
                    MemberShipNo = s.MemberShipNo!,
                    MemberName = s.Member?.Name,
                    //TotalPaymentAmount = data.Sum(s1 => s1.PaymentFees) + data.Sum(s2 => s2.LateFees),
                    ActualPaymentDate = s.ActualPaymentDate,

                    SubscriptionYear = s.SubscriptionYear,
                    SubscriptionName = s.SubscriptionName,
                    SubscriptionMonth = s.SubscriptionMonth,
                    PaymentAmount = s.PaymentFees,
                    LateAmount = s.LateFees,
                   // LateFePer = s.LateFees,
                    IsChecked = s.IsPaid,
                  


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
