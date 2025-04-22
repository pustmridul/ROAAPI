using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Domain.Entities.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace MemApp.Infrastructure.Services
{

    public class SubscriptionImp : ISubscription
    {
        private readonly IServiceProvider _serviceProvider;
        public SubscriptionImp(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

        }
        public async Task<bool> MonthlyDueGenerator(DateTime syncDate)
        {

            using (var scope = _serviceProvider.CreateScope())
            {

                var _context = scope.ServiceProvider.GetRequiredService<IMemDbContext>();


                var result = new Result();
                var dataList = new List<SubscriptionDueTemp>();
                DateTime startDate = new DateTime(2024, 6, 1);
                var memberList = await _context.RegisterMembers
                       .Include(i => i.MemberTypes)
                       .Include(i => i.MemberActiveStatus)
                       .Where(q => q.IsActive && q.IsMasterMember == true && q.MemberStatusId != 16 && q.PaidTill < syncDate && q.MemberTypes.IsSubscribed)
                       .AsNoTracking()
                       .ToListAsync();

                var subChargeList = await _context.SubscriptionFees
                      .Include(i => i.SubscriptionMode)
                      .Where(q => q.IsActive && q.StartDate.Date <= syncDate)
                      .OrderBy(c => c.StartDate)
                      .AsNoTracking()
                      .ToListAsync();


                var lastSyncDate = await _context.SubscriptionDueTemps
                    .OrderByDescending(q => q.SyncDate)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
                if (lastSyncDate == null)
                {
                    var s = subChargeList.OrderBy(q => q.StartDate).FirstOrDefault()?.StartDate;
                    if (s != null)
                    {
                        startDate = s ?? startDate;
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
                        var dueSubscription = subChargeList.Where(q => q.StartDate > m.PaidTill).ToList();

                        if (dueSubscription.Any())
                        {
                            foreach (var sub in dueSubscription)
                            {
                                if (sub.StartDate.Date == syncDate)
                                {
                                    if (syncDate.Month == 1 || syncDate.Month == 4 || syncDate.Month == 7 || syncDate.Month == 10)
                                    {

                                        var obj = new SubscriptionDueTemp()
                                        {
                                            MemberShipNo = m.MembershipNo!,
                                            MemberId = m.Id,
                                            ActualPaymentDate = sub.StartDate,
                                            SubscriptionFeesId = sub.Id,
                                            SyncDate = syncDate,
                                            GenerateDate = new DateTime(syncDate.Year, syncDate.Month, 1),
                                            LateFeePer = sub.LateFee ?? 0,
                                            SubscriptionYear = sub.SubscribedYear ?? "",
                                            PaymentAmount = (m.MemberActiveStatus?.Name == "Abroad" || m.MemberActiveStatus?.Name == "Absentee") ? ((sub.AbroadFee ?? 0) / 100) * sub.SubscriptionFee : sub.SubscriptionFee,
                                            LateAmount = (m.MemberActiveStatus?.Name == "Abroad" || m.MemberActiveStatus?.Name == "Absentee") ?
                            (sub.SubscriptionFee * ((sub.AbroadFee ?? 0) / 100) * (sub.LateFee ?? 0)) / 100 : ((sub.LateFee ?? 0) / 100) * sub.SubscriptionFee,
                                            SubscriptionName = sub.SubscriptionMode.Name,
                                            AbroadFeeAmount = sub.SubscriptionFee * ((sub.AbroadFee ?? 0) / 100),
                                            AbroadFeePer = sub.AbroadFee ?? 0,
                                            IsQBSync = false,
                                            IsPaid = false,
                                            QBCusName = m.QBCusName,
                                            QBSyncDate = null,
                                        };
                                        dataList.Add(obj);
                                    }
                                }

                                else
                                {
                                    var obj = new SubscriptionDueTemp()
                                    {
                                        MemberShipNo = m.MembershipNo!,
                                        ActualPaymentDate = sub.StartDate,
                                        SubscriptionFeesId = sub.Id,
                                        MemberId = m.Id,
                                        SyncDate = syncDate,
                                        GenerateDate = new DateTime(syncDate.Year, syncDate.Month, 1),
                                        SubscriptionYear = sub.SubscribedYear ?? "",
                                        PaymentAmount = 0,
                                        LateFeePer = sub.LateFee ?? 0,
                                        LateAmount = (m.MemberActiveStatus?.Name == "Abroad" || m.MemberActiveStatus?.Name == "Absentee") ?
                         (sub.SubscriptionFee * ((sub.AbroadFee ?? 0) / 100) * (sub.LateFee ?? 0)) / 100 : ((sub.LateFee ?? 0) / 100) * sub.SubscriptionFee,
                                        SubscriptionName = sub.SubscriptionMode.Name,
                                        AbroadFeeAmount = 0,
                                        AbroadFeePer = sub.AbroadFee ?? 0,
                                        IsQBSync = false,
                                        IsPaid = false,
                                        QBCusName = m.QBCusName,
                                        QBSyncDate = null,
                                    };
                                    dataList.Add(obj);
                                }


                            }

                        }

                    }

                    try
                    {
                        _context.SubscriptionDueTemps.AddRange(dataList);
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
    }
}
