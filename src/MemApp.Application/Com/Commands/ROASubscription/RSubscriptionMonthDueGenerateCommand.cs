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

namespace ResApp.Application.Com.Commands.Subscription
{
    public class RSubscriptionMonthDueGenerateCommand : IRequest<Result>
    {
        // public DateTime SyncDate { get; set; } = DateTime.UtcNow;
        public DateTime SyncDate { get; set; } = DateTime.Now; // TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Asia/Dhaka"));

    }

    public class RSubscriptionMonthDueGenerateCommandHandler : IRequestHandler<RSubscriptionMonthDueGenerateCommand, Result>
    {
        private readonly IMemDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        public RSubscriptionMonthDueGenerateCommandHandler(IMemDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }
        public async Task<Result> Handle(RSubscriptionMonthDueGenerateCommand request, CancellationToken cancellationToken)
        {
            var result = new Result();
            var userName = _currentUserService.Current().UserName;

            var user =_context.Users.Where(x=>x.UserName == userName).FirstOrDefault();

            // var dataList = new List<SubscriptionDueTemp>();
            var dataList = new List<RoSubscriptionDueTemp>();
            DateTime startDate = new DateTime(2025, 01, 1);
            var memberList = await _context.MemberRegistrationInfos
                  // .Include(i => i.MemberTypes)
                 //  .Include(i => i.MemberActiveStatus)
                 //  .Where(q => q.IsActive && q.IsApproved && q.PaidTill!.Value.Date < request.SyncDate.Date)
                   .Where(q => q.IsActive && q.IsApproved && q.PaidTill!.Value.Date < request.SyncDate.Date)
                   .AsNoTracking()
                   .ToListAsync(cancellationToken);

            //var subChargeList = await _context.SubscriptionFees
            //                 .Include(i => i.SubscriptionMode)
            //                 .Where(q => q.IsActive && q.StartDate.Date <= request.SyncDate)
            //                 .OrderBy(c => c.StartDate)
            //                 .AsNoTracking()
            //                 .ToListAsync(cancellationToken);

            //var subChargeList = await _context.ROASubscriptionFees
            //                //.Include(i => i.SubscriptionMode)
            //                .Where(q => q.IsActive && q.StartDate.Date <= request.SyncDate)
            //                .OrderBy(c => c.StartDate)
            //                .AsNoTracking()
            //                .ToListAsync(cancellationToken);

            List<DateTime> months = GetMonthList(startDate, request.SyncDate);

            //var lastSyncDate = await _context.SubscriptionDueTemps
            //    .OrderByDescending(q => q.SyncDate)
            //    .AsNoTracking()
            //    .FirstOrDefaultAsync(cancellationToken);

            var lastSyncDate = await _context.RoSubscriptionDueTemps
               .OrderByDescending(q => q.SyncDate)
               .AsNoTracking()
               .FirstOrDefaultAsync(cancellationToken);
            if (lastSyncDate == null)
            {
                //  var s = subChargeList.OrderBy(q => q.StartDate).FirstOrDefault()?.StartDate;
                var firstMonth = months.OrderBy(q => q).FirstOrDefault();
                if (firstMonth != null)
                {
                   // startDate = s ?? startDate;
                    startDate = firstMonth;
                }
            }
            else
            {
                startDate = lastSyncDate.SyncDate;
            }

            if (memberList.Count > 0)
            {
                foreach (var m in memberList)
                {
                    if (m.PaidTill > startDate)
                    {
                        startDate = m.PaidTill ?? startDate;
                    }
                   // var dueSubscription = subChargeList.Where(q => q.StartDate > m.PaidTill).ToList();
                    var dueSubscription = months.Where(q => q > m.PaidTill).ToList();

                    if (dueSubscription.Any())
                    {
                        foreach (var sub in dueSubscription)
                        {
                           // if (sub.StartDate.Date == request.SyncDate.Date)
                            if (sub == request.SyncDate.Date)
                            {

                                //  if (request.SyncDate.Month == 1 || request.SyncDate.Month == 4 || request.SyncDate.Month == 7 || request.SyncDate.Month == 10)
                                //  {
                                var checkExist = _context.RoSubscriptionDueTemps
                                                             .AsNoTracking()
                                                             .Any(x => x.MemberId == m.Id && x.SubscriptionMonth == sub && x.SubscriptionYear == sub.Year);
                                if(!checkExist)
                                {
                                    var obj = new RoSubscriptionDueTemp()
                                    {
                                        MemberShipNo = m.MemberShipNo!,
                                        MemberId = m.Id,
                                        ActualPaymentDate = sub,//  sub.StartDate,
                                        SubscriptionFeeId = 1,// sub.Id,
                                        SyncDate = request.SyncDate,
                                        GenerateDate = new DateTime(request.SyncDate.Year, request.SyncDate.Month, 1),
                                        LateFeePer = 0,// sub.LateFee ?? 0,
                                                       //  SubscriptionYear = sub.SubscribedYear ?? "",
                                        SubscriptionYear = sub.Year,  
                                        PaymentAmount = m.SubscriptionFee.GetValueOrDefault(),//  m.SubscriptionFee?? sub.SubscriptionFee,
                                        LateAmount = 0, //sub.LateFee.GetValueOrDefault(),
                                                         //  SubscriptionName = sub.SubscriptionMode.Name,
                                        SubscriptionName = "Monthly",// sub.Title,                                                                          
                                        IsQBSync = false,
                                        // IsPaid = false ,
                                        IsPaid = _context.ROASubscriptionPaymentDetail.Any(x => x.SubscriptionMonth == sub   // sub.SubscribedMonth
                                                        && x.SubscriptionYear == sub.Year // sub.SubscribedYear
                                                        && x.MemberId == m.Id),
                                        QBCusName = m.Name,
                                        QBSyncDate = null,
                                        GeneratedBy = "Manually",
                                        SubscriptionMonth = sub, //   sub.SubscribedMonth,
                                        CreatedBy = user!.Id,
                                        CreatedByName = user.Name,
                                        CreatedOn = DateTime.Now,
                                    };
                                    dataList.Add(obj);
                                    //  }
                                }


                            }

                            else
                            {
                                var checkExist = _context.RoSubscriptionDueTemps
                                                             .AsNoTracking()
                                                             .Any(x => x.MemberId == m.Id && x.SubscriptionMonth == sub && x.SubscriptionYear == sub.Year);
                                if (!checkExist)
                                {
                                    var obj = new RoSubscriptionDueTemp()
                                    {
                                        MemberShipNo = m.MemberShipNo!,
                                        ActualPaymentDate = sub, // sub.StartDate,
                                        SubscriptionFeeId = 1, // sub.Id,
                                        MemberId = m.Id,
                                        SyncDate = request.SyncDate,
                                        GenerateDate = new DateTime(request.SyncDate.Year, request.SyncDate.Month, 1),
                                        SubscriptionYear = sub.Year, // sub.SubscribedYear ,
                                        PaymentAmount = m.SubscriptionFee.GetValueOrDefault(),
                                        LateFeePer = 0, // sub.LateFee ?? 0,
                                                        //               LateAmount = (m.MemberActiveStatus?.Name == "Abroad" || m.MemberActiveStatus?.Name == "Absentee") ?
                                                        //(sub.SubscriptionFee * ((sub.AbroadFee ?? 0) / 100) * (sub.LateFee ?? 0)) / 100 : ((sub.LateFee ?? 0) / 100) * sub.SubscriptionFee,
                                        LateAmount = 0, // sub.LateFee.GetValueOrDefault(),
                                        SubscriptionName = "Montly", // sub.Title,

                                        IsQBSync = false,
                                        IsPaid = _context.ROASubscriptionPaymentDetail.Any(x => x.SubscriptionMonth == sub // sub.SubscribedMonth 
                                                            && x.SubscriptionYear == sub.Year //  sub.SubscribedYear 
                                                            && x.MemberId == m.Id),
                                        QBCusName = m.Name,
                                        QBSyncDate = null,
                                        GeneratedBy = "Manually",
                                        SubscriptionMonth = sub, // sub.SubscribedMonth,
                                        CreatedBy = user!.Id,
                                        CreatedByName = user.UserName,
                                        CreatedOn = DateTime.Now,
                                    };
                                    dataList.Add(obj);
                                }
                            }


                        }

                    }

                }

                try
                {
                    _context.RoSubscriptionDueTemps.AddRange(dataList);
                    await _context.SaveAsyncOnly();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

            }
            result.HasError = false;
            result.Messages.Add(" Date : " + request.SyncDate + "Sync success");
            return result;
        }

        private DateTime GetCurrentQuater()
        {
            DateTime currentDate = DateTime.Now;
            int quarter = (currentDate.Month - 1) / 3 + 1;
            DateTime lastDateOfQuarter = new DateTime(currentDate.Year, quarter * 3, DateTime.DaysInMonth(currentDate.Year, quarter * 3));

            return lastDateOfQuarter;
        }

        static List<DateTime> GetMonthList(DateTime start, DateTime end)
        {
            return Enumerable.Range(0, ((end.Year - start.Year) * 12 + end.Month - start.Month) + 1)
                             // .Select(i => start.AddMonths(i).ToString("yyyy-MM"))
                             .Select(i => new DateTime(start.Year, start.Month, 1).AddMonths(i))
                             .ToList();
        }


    }
}
