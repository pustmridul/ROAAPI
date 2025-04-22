using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Subscription.Model;
using MemApp.Domain.Entities.subscription;
using Microsoft.EntityFrameworkCore;
using ResApp.Application.Com.Commands.ROAPayment.Models;
using ResApp.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ResApp.Application.ROA.RoaSubcription.Queries
{

    public class GetAdvanceListByROMemberIdQuery : IRequest<ListResult<MemberSubPaymentRes>>
    {
        public int MemberId { get; set; }
    }

    public class GetAdvanceListByROMemberIdQueryHandler : IRequestHandler<GetAdvanceListByROMemberIdQuery, ListResult<MemberSubPaymentRes>>
    {
        private readonly IMemDbContext _context;
        public GetAdvanceListByROMemberIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<ListResult<MemberSubPaymentRes>> Handle(GetAdvanceListByROMemberIdQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var result = new ListResult<MemberSubPaymentRes>
                {
                    Data = new List<MemberSubPaymentRes>()
                };

                var memberObj = await _context.MemberRegistrationInfos
                   .AsNoTracking()
                   .SingleOrDefaultAsync(s => s.Id == request.MemberId && s.IsApproved, cancellationToken);
                //  .Select(s=> new {s.Id, s.PaidTill, s.Phone, s.Email, s.MembershipNo, s.MemberTypeId});



                if (memberObj != null)
                {



                    var dueExist = await _context.RoSubscriptionDueTemps
                                       .AsNoTracking()
                                       .AnyAsync(q => q.MemberId == request.MemberId && !q.IsPaid, cancellationToken);


                    if (dueExist)
                    {
                        result.HasError = true;
                        result.Messages.Add("Please pay the due first");
                        return result;
                    }

                    if (memberObj.PaidTill == null)
                    {
                        result.HasError = true;
                        result.Messages.Add("Please set the subscription first!");
                        return result;
                    }

                    var lastSyncDate = await _context.RoSubscriptionDueTemps
                                             .OrderByDescending(q => q.SyncDate)
                                             .AsNoTracking()
                                             .FirstOrDefaultAsync();

                    if (lastSyncDate == null)
                    {
                        result.HasError = true;
                        result.Messages?.Add("Please generate the subscription first!");
                        return result;
                    }

                    List<DateTime> next12Months = GetNext12Months(memberObj.PaidTill.GetValueOrDefault()).OrderBy(x => x).ToList();
                    int n = 0;
                    foreach (var month in next12Months)
                    {
                        //Console.WriteLine(month.ToString("yyyy-MM-dd"));
                        n++;
                        var paymentRes = new MemberSubPaymentRes
                        {
                            MemberId = memberObj.Id,
                            PaymentAmount = memberObj.SubscriptionFee.GetValueOrDefault(),
                            // ActualPaymentDate=s.ActualPaymentDate,
                            LateAmount = 0,
                            Id = n,
                            SubscriptionMonth = month,
                            SubscriptionName = "Monthly",
                            SubscriptionYear = month.Year,
                            MemberName = memberObj.Name,
                            IsChecked = false
                            // PaymentDate=s.
                        };
                        result.Data.Add(paymentRes);
                    }

                    result.HasError = false;

                }
                else
                {
                    result.HasError = true;
                    result.Messages?.Add("Data Not Found");
                }



                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        static List<DateTime> GetNext12Months(DateTime start)
        {
            List<DateTime> months = new List<DateTime>();

            for (int i = 1; i <= 12; i++) // Start from next month
            {
                months.Add(start.AddMonths(i));
            }

            return months;
        }

    }
}
