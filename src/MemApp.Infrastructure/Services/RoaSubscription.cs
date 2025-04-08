using Azure.Core;
using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Domain.Entities;
using MemApp.Domain.Entities.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Res.Domain.Entities.ROAPayment;
using System.Threading;


namespace MemApp.Infrastructure.Services
{

    public class RoaSubscription : IROASubscription
    {
        private readonly IServiceProvider _serviceProvider;
        public RoaSubscription(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

        }
        public async Task<bool> MonthlyDueGenerator(DateTime syncDate)
        {

            using (var scope = _serviceProvider.CreateScope())
            {

                var _context = scope.ServiceProvider.GetRequiredService<IMemDbContext>();


                var result = new Result();
                var dataList = new List<RoSubscriptionDueTemp>();
                DateTime startDate = new DateTime(2025, 1, 1);
                var memberList = await _context.MemberRegistrationInfos
                       //.Include(i => i.MemberTypes)
                      // .Include(i => i.MemberActiveStatus)
                       .Where(q => q.IsActive && q.IsApproved == true /*&& q.MemberStatusId != 16*/ && q.PaidTill < syncDate /*&& q.MemberTypes.IsSubscribed*/)
                       .AsNoTracking()
                       .ToListAsync();

                //var subChargeList = await _context.SubscriptionFees
                //      .Include(i => i.SubscriptionMode)
                //      .Where(q => q.IsActive && q.StartDate.Date <= syncDate)
                //      .OrderBy(c => c.StartDate)
                //      .AsNoTracking()
                //      .ToListAsync();
                List<DateTime> months = GetMonthList(startDate, syncDate);

                var lastSyncDate = await _context.RoSubscriptionDueTemps
                .OrderByDescending(q => q.SyncDate)
                .AsNoTracking()
                .FirstOrDefaultAsync();
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
                        //  var dueSubscription = subChargeList.Where(q => q.StartDate > m.PaidTill).ToList();
                        var dueSubscription = months.Where(q => q > m.PaidTill).ToList();

                        if (dueSubscription.Any())
                        {
                            foreach (var sub in dueSubscription)
                            {
                                // if (sub == syncDate)
                                // {
                                // //    if (syncDate.Month == 1 || syncDate.Month == 4 || syncDate.Month == 7 || syncDate.Month == 10)
                                ////     {

                                //         var obj = new SubscriptionDueTemp()
                                //         {
                                //            // MemberShipNo = m.MembershipNo,
                                //             MemberShipNo=m.MemberShipNo!,
                                //             MemberId = m.Id,
                                //             ActualPaymentDate = sub.StartDate,
                                //             SubscriptionFeesId = sub.Id,
                                //             SyncDate = syncDate,
                                //             GenerateDate = new DateTime(syncDate.Year, syncDate.Month, 1),
                                //             LateFeePer = sub.LateFee ?? 0,
                                //             SubscriptionYear = sub.SubscribedYear ?? "",
                                //             //  PaymentAmount = (m.MemberActiveStatus?.Name == "Abroad" || m.MemberActiveStatus?.Name == "Absentee") ? ((sub.AbroadFee ?? 0) / 100) * sub.SubscriptionFee : sub.SubscriptionFee,
                                //             //  LateAmount = (m.MemberActiveStatus?.Name == "Abroad" || m.MemberActiveStatus?.Name == "Absentee") ?
                                //             //  (sub.SubscriptionFee * ((sub.AbroadFee ?? 0) / 100) * (sub.LateFee ?? 0)) / 100 : ((sub.LateFee ?? 0) / 100) * sub.SubscriptionFee,
                                //             PaymentAmount = m.SubscriptionFee.GetValueOrDefault(),
                                //             LateAmount = (m.SubscriptionFee.GetValueOrDefault()/100) * 10,
                                //             SubscriptionName = sub.SubscriptionMode.Name,
                                //            // AbroadFeeAmount = sub.SubscriptionFee * ((sub.AbroadFee ?? 0) / 100),
                                //            // AbroadFeePer = sub.AbroadFee ?? 0,
                                //             IsQBSync = false,
                                //             IsPaid = false,
                                //            // QBCusName = m.QBCusName,
                                //             QBCusName = m.Name,
                                //             QBSyncDate = null,
                                //         };
                                //         dataList.Add(obj);
                                //     }
                                // }

                                // else
                                // {
                                var checkExist = _context.RoSubscriptionDueTemps
                                                             .AsNoTracking()
                                                             .Any(x => x.MemberId == m.Id && x.SubscriptionMonth == sub && x.SubscriptionYear == sub.Year);
                                if (!checkExist)
                                {
                                    var obj = new RoSubscriptionDueTemp()
                                    {
                                        MemberShipNo = m.MemberShipNo!,
                                        MemberId = m.Id,
                                        ActualPaymentDate = sub,//  sub.StartDate,
                                        SubscriptionFeeId = 1,// sub.Id,
                                        SyncDate = syncDate,
                                        GenerateDate = new DateTime(syncDate.Year, syncDate.Month, 1),
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
                                        GeneratedBy = "Systematically",
                                        SubscriptionMonth = sub, //   sub.SubscribedMonth,
                                        //CreatedBy = user!.Id,
                                        //CreatedByName = user.Name,
                                        //CreatedOn = DateTime.UtcNow,
                                    };
                                    dataList.Add(obj);
                                }
                                   
                                }


                         //   }

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

            }

            return false;
        }
        private int GetMonth(DateTime payementDate)
        {
            if (payementDate > DateTime.Now)
            {
                return 0;
            }
            else
            {
                int monthsDifference = (DateTime.Now.Year - payementDate.Year) * 12 + DateTime.Now.Month - payementDate.Month + 1;
                return monthsDifference;
            }

        }

        private List<DateTime> GetMonthList(DateTime start, DateTime end)
        {
            return Enumerable.Range(0, ((end.Year - start.Year) * 12 + end.Month - start.Month) + 1)
                             // .Select(i => start.AddMonths(i).ToString("yyyy-MM"))
                             .Select(i => new DateTime(start.Year, start.Month, 1).AddMonths(i))
                             .ToList();
        }
    }
}
