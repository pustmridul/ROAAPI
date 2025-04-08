using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Subscription.Model;
using MemApp.Application.Mem.Subscription.Queries;
using Microsoft.EntityFrameworkCore;
using ResApp.Application.Com.Commands.ROAPayment.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResApp.Application.Com.Queries.ROSubcriptionPaymentReport
{
    public class GetRoSubscriptionPaymentQuery : IRequest<Result<PaymentReport>>
    {
        public string PaymentNo { get; set; } = string.Empty;
    }

    public class GetRoSubscriptionPaymentQueryHandler : IRequestHandler<GetRoSubscriptionPaymentQuery, Result<PaymentReport>>
    {
        private readonly IMemDbContext _context;
        public GetRoSubscriptionPaymentQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PaymentReport>> Handle(GetRoSubscriptionPaymentQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = new Result<PaymentReport>()
                {
                    Data= new PaymentReport
                    {
                        PaymentDetails= new List<MemberSubPaymentRes>()
                    }
                };

               
         

                var data = await _context.ROASubscriptionPayments
                    .Include(i => i.Member)
                    .Include(i => i.SubscriptionPaymentDetails)
                    .Where(q => q.PaymentNo == request.PaymentNo)
                    .OrderByDescending(q => q.ActualPaymentDate)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cancellationToken);

                if (data == null)
                {
                    result.HasError = true;
                    result.Messages?.Add("Data Not Found");
                }

                // Extract SubscriptionMonths
                var allMonths = await _context.ROASubscriptionPaymentDetail.Where(x=> x.PaymentNo == request.PaymentNo)
                                    .OrderBy(d => d.SubscriptionMonth) // Order by SubscriptionMonth ASC
                                   // .Select(d => d.SubscriptionMonth)
                                    .ToListAsync();

                if (allMonths.Count > 1)
                {
                    var firstMonth = allMonths.FirstOrDefault(); // First (Earliest)
                    var lastMonth = allMonths.LastOrDefault(); // Last (Latest)
                    result.Data.SubscriptionDetails = firstMonth!.SubscriptionMonth.ToString("MMMM, yyyy") + " - " + lastMonth!.SubscriptionMonth.ToString("MMMM, yyyy");
                }

                if (allMonths.Count == 1)
                {
                    var firstMonth = allMonths.FirstOrDefault(); // First (Earliest)
                   // var lastMonth = allMonths.LastOrDefault(); // Last (Latest)
                    result.Data.SubscriptionDetails = firstMonth!.SubscriptionMonth.ToString("MMMM, yyyy");
                }

                result.Data.PaymentDate = data!.ActualPaymentDate.GetValueOrDefault();
                result.Data.MemberName = data!.Member.Name!;
                result.Data.MemberShipNo = data.MemberShipNo;
                result.Data.PaymentNo = data.PaymentNo;
                result.Data.TotalPaymentAmount = data!.TotalAmount.HasValue
                                                         ? data.TotalAmount.Value
                                                         : allMonths.Sum(x => x.PaymentFees);


                result.HasError = false;
                result.Data.PaymentDetails = allMonths.Select(s => new MemberSubPaymentRes
                {
                    Id = s.Id,
                    PaymentDate = s.ActualPaymentDate,
                    PaymentNo = s.PaymentNo,
                    MemberShipNo = s.MemberShipNo!,
                    MemberName = s.Member?.Name,
                 //   TotalPaymentAmount = s.TotalAmount,// data.Sum(s1 => s1.TotalAmount), // + data.Sum(s2 => s2.LateAmount),
                    ActualPaymentDate = s.ActualPaymentDate,


                   // PaymentYear = s. ?? "",
                 //   SubscriptionDetails= firstMonth.ToString("MMMM yyyy") + "-" + lastMonth.ToString("MMMM yyyy"),
                  //  SubscriptionMonth=s
                    SubscriptionName = s.SubscriptionName,
                    SubscriptionMonth = s.SubscriptionMonth,
                    SubscriptionYear = s.SubscriptionYear,
                    PaymentAmount = s.PaymentFees,
                    LateAmount = s.LateFees,// s.LateAmount,
                   // LateFePer = 0,// s.LateFeePer,
                 //   IsChecked = s.IsPaid,



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
