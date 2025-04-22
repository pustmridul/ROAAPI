using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Subscription.Command;
using MemApp.Domain.Entities.Payment;
using Microsoft.EntityFrameworkCore;
using Res.Domain.Entities.ROAPayment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResApp.Application.ROA.RoaSubcription.Command
{
    public class RSubscriptionDueByMemberCommand : IRequest<Result>
    {
        // public DateTime SyncDate { get; set; } = DateTime.UtcNow;
        //  public DateTime SyncDate { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Asia/Dhaka"));

        public int MemberId { get; set; }
    }

    public class RSubscriptionDueByMemberCommandHandler : IRequestHandler<RSubscriptionDueByMemberCommand, Result>
    {
        private readonly IMemDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        public RSubscriptionDueByMemberCommandHandler(IMemDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }
        public async Task<Result> Handle(RSubscriptionDueByMemberCommand request, CancellationToken cancellationToken)
        {
            var result = new Result();
            var userName = _currentUserService.Current().UserName;

            var user = _context.Users.Where(x => x.UserName == userName).FirstOrDefault();

            // var dataList = new List<SubscriptionDueTemp>();
            var dataList = new List<RoSubscriptionDueTemp>();
            DateTime startDate = new DateTime(2025, 01, 1);

            var member = await _context.MemberRegistrationInfos
                   .Where(q => q.Id == request.MemberId && q.IsActive && q.IsApproved && q.PaidTill != null)
                   .AsNoTracking()
                   .FirstOrDefaultAsync(cancellationToken);

            if (member == null)
            {
                result.HasError = true;
                result.Messages?.Add("Member not found!");
                return result;
            }


            var lastSyncDate = await _context.RoSubscriptionDueTemps
              .OrderByDescending(q => q.SyncDate)
              .AsNoTracking()
              .FirstOrDefaultAsync();

            if (lastSyncDate == null)
            {
                //lastSyncDate!.SyncDate = startDate;
                //result.HasError = true;
                //result.Messages?.Add("Please generate the subscription first!");
                //return result;
            }

            List<DateTime> months = new List<DateTime>();

            if (lastSyncDate != null)
            {

            }

            months = GetMonthList(member!.PaidTill.GetValueOrDefault(), DateTime.Now);

            try
            {
                foreach (var m in months)
                {



                    var checkExist = _context.RoSubscriptionDueTemps
                                                 .AsNoTracking()
                                                 .Any(x => x.MemberId == member.Id && x.SubscriptionMonth == m && x.SubscriptionYear == m.Year);
                    if (!checkExist)
                    {
                        var obj = new RoSubscriptionDueTemp()
                        {
                            MemberShipNo = member.MemberShipNo!,
                            MemberId = member.Id,
                            ActualPaymentDate = m,//  sub.StartDate,
                            SubscriptionFeeId = 1,// sub.Id,
                            SyncDate = lastSyncDate == null ? startDate : lastSyncDate.SyncDate, // request.SyncDate,
                            GenerateDate = DateTime.Now, // new DateTime(request.SyncDate.Year, request.SyncDate.Month, 1),
                            LateFeePer = 0,
                            SubscriptionYear = m.Year,
                            PaymentAmount = member.SubscriptionFee.GetValueOrDefault(),//  m.SubscriptionFee?? sub.SubscriptionFee,
                            LateAmount = 0,
                            SubscriptionName = "Monthly",// sub.Title,                                                                          
                            IsQBSync = false,
                            // IsPaid = false ,
                            IsPaid = false,
                            QBCusName = member.Name,
                            QBSyncDate = null,
                            GeneratedBy = "Manually",
                            SubscriptionMonth = m, //   sub.SubscribedMonth,

                        };
                        dataList.Add(obj);

                    }

                }
                _context.RoSubscriptionDueTemps.AddRange(dataList);
                await _context.SaveAsyncOnly();

                result.HasError = false;
                //  result.Messages.Add(" Date : " + request.SyncDate + "Sync success");
                return result;

            }


            catch (Exception ex)
            {
                result.HasError = true;
                //  result.Messages.Add(" Date : " + request.SyncDate + "Sync success");
                return result;
                //  throw new Exception(ex.Message);
            }


        }



        static List<DateTime> GetMonthList(DateTime start, DateTime end)
        {
            return Enumerable.Range(1, (end.Year - start.Year) * 12 + end.Month - start.Month)
                             // .Select(i => start.AddMonths(i).ToString("yyyy-MM"))
                             .Select(i => new DateTime(start.Year, start.Month, 1).AddMonths(i))
                             .ToList();
        }


    }
}
