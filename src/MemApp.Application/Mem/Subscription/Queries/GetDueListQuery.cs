using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Domain.Entities.Payment;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Subscription.Queries
{

    public class GetDueListQuery : IRequest<Result>
    {
        public DateTime SyncDate { get; set; } = DateTime.Now;
    }

    public class GetDueListQueryHandler : IRequestHandler<GetDueListQuery, Result>
    {
        private readonly IMemDbContext _context;
        public GetDueListQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(GetDueListQuery request, CancellationToken cancellationToken)
        {
            bool q1 = false;
            bool q2 = false;
            bool q3 = false;
            bool q4 = false;
            var result = new Result();
            var dataList = new List<SubscriptionDueTemp>();
            DateTime startDate = DateTime.Now;
            var memberList = await _context.RegisterMembers
                   .Include(i => i.MemberTypes)
                   .Include(i => i.MemberActiveStatus)
                   .Where(q => q.IsActive && q.IsMasterMember == true && q.PaidTill < request.SyncDate && q.MemberTypes.IsSubscribed)
                   .AsNoTracking()
                   .ToListAsync(cancellationToken);

            var subChargeList = await _context.SubscriptionFees
                             .Include(i => i.SubscriptionMode)
                             .Where(q => q.IsActive && q.StartDate.Date <= request.SyncDate)
                             .OrderBy(c => c.StartDate)
                             .AsNoTracking()
                             .ToListAsync(cancellationToken);

            var lastSyncDate = await _context.SubscriptionDueTemps
                .OrderByDescending(q => q.SyncDate)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
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
                 //   var dueSubscription = subChargeList.Where(q => q.EndDate <= request.SyncDate).ToList();

                    if (subChargeList.Any())
                    {
                        foreach (var sub in subChargeList)
                        {
                            q1 = false;
                            q2 = false;
                            q3 = false;
                            q4 = false;
                            for (var subm = sub.StartDate; subm <= request.SyncDate; subm = subm.AddMonths(1))
                            {
                                if (subm.Month == 1 && sub.SubscriptionMode.Name == "Q1" && !q1)
                                {
                                    q1 = true;
                                    var obj = new SubscriptionDueTemp()
                                    {
                                        MemberShipNo = m.MembershipNo,
                                        MemberId = m.Id,
                                        ActualPaymentDate = sub.StartDate,
                                        SubscriptionFeesId = sub.Id,
                                        SyncDate = request.SyncDate,
                                        GenerateDate = subm,
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
                                else if (subm.Month == 4 && sub.SubscriptionMode.Name == "Q2" && !q2)
                                {
                                    q2 = true;
                                    var obj = new SubscriptionDueTemp()
                                    {
                                        MemberShipNo = m.MembershipNo,
                                        MemberId = m.Id,
                                        ActualPaymentDate = sub.StartDate,
                                        SubscriptionFeesId = sub.Id,
                                        SyncDate = request.SyncDate,
                                        GenerateDate = subm,
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
                                else if (subm.Month == 7 && sub.SubscriptionMode.Name == "Q3" && !q3)
                                {
                                    q3 = true;
                                    var obj = new SubscriptionDueTemp()
                                    {
                                        MemberShipNo = m.MembershipNo,
                                        MemberId = m.Id,
                                        ActualPaymentDate = sub.StartDate,
                                        SubscriptionFeesId = sub.Id,
                                        SyncDate = request.SyncDate,
                                        GenerateDate = subm,
                                        LateFeePer = sub.LateFee ?? 0,
                                        SubscriptionYear = sub.SubscribedYear ?? "",
                                        PaymentAmount = (m.MemberActiveStatus?.Name == "Abroad" || m.MemberActiveStatus?.Name == "Absentee") ? ((sub.AbroadFee ?? 0) / 100) * sub.SubscriptionFee : sub.SubscriptionFee,
                                        LateAmount = (m.MemberActiveStatus?.Name == "Abroad" || m.MemberActiveStatus?.Name == "Absentee") ?
                        (sub.SubscriptionFee * ((sub.AbroadFee ?? 0) / 100) * (sub.LateFee ?? 0)) / 100 : ((sub.LateFee ?? 0) / 100) * sub.SubscriptionFee,
                                        SubscriptionName = sub.SubscriptionMode.Name,
                                        AbroadFeeAmount = sub.SubscriptionFee * ((sub.AbroadFee ?? 0) / 100),
                                        AbroadFeePer = sub.AbroadFee ?? 0
                                    };
                                    dataList.Add(obj);
                                }
                                else if (subm.Month == 10 && sub.SubscriptionMode.Name == "Q4" && !q4)
                                {
                                    q4 = true;
                                    var obj = new SubscriptionDueTemp()
                                    {
                                        MemberShipNo = m.MembershipNo,
                                        MemberId = m.Id,
                                        ActualPaymentDate = sub.StartDate,
                                        SubscriptionFeesId = sub.Id,
                                        SyncDate = request.SyncDate,
                                        GenerateDate = subm,
                                        LateFeePer = sub.LateFee ?? 0,
                                        SubscriptionYear = sub.SubscribedYear ?? "",
                                        PaymentAmount = (m.MemberActiveStatus?.Name == "Abroad" || m.MemberActiveStatus?.Name == "Absentee") ? ((sub.AbroadFee ?? 0) / 100) * sub.SubscriptionFee : sub.SubscriptionFee,
                                        LateAmount = (m.MemberActiveStatus?.Name == "Abroad" || m.MemberActiveStatus?.Name == "Absentee") ?
                        (sub.SubscriptionFee * ((sub.AbroadFee ?? 0) / 100) * (sub.LateFee ?? 0)) / 100 : ((sub.LateFee ?? 0) / 100) * sub.SubscriptionFee,
                                        SubscriptionName = sub.SubscriptionMode.Name,
                                        AbroadFeeAmount = sub.SubscriptionFee * ((sub.AbroadFee ?? 0) / 100),
                                        AbroadFeePer = sub.AbroadFee ?? 0
                                    };
                                    dataList.Add(obj);
                                }
                                else
                                {
                                    var obj = new SubscriptionDueTemp()
                                    {
                                        MemberShipNo = m.MembershipNo,
                                        ActualPaymentDate = sub.StartDate,
                                        SubscriptionFeesId = sub.Id,
                                        MemberId = m.Id,
                                        SyncDate = request.SyncDate,
                                        GenerateDate = subm,
                                        SubscriptionYear = sub.SubscribedYear ?? "",
                                        PaymentAmount = 0,
                                        LateFeePer = sub.LateFee ?? 0,
                                        LateAmount = (m.MemberActiveStatus?.Name == "Abroad" || m.MemberActiveStatus?.Name == "Absentee") ?
                         (sub.SubscriptionFee * ((sub.AbroadFee ?? 0) / 100) * (sub.LateFee ?? 0)) / 100 : ((sub.LateFee ?? 0) / 100) * sub.SubscriptionFee,
                                        SubscriptionName = sub.SubscriptionMode.Name,
                                        AbroadFeeAmount = 0,
                                        AbroadFeePer = sub.AbroadFee ?? 0
                                    };
                                    dataList.Add(obj);
                                }

                            }

                        }

                    }


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
            result.HasError = false;
            result.Messages.Add("success");
            return result;
        }
      
        private DateTime GetCurrentQuater()
        {
            DateTime currentDate = DateTime.Now;
            int quarter = (currentDate.Month - 1) / 3 + 1;
            DateTime lastDateOfQuarter = new DateTime(currentDate.Year, quarter * 3, DateTime.DaysInMonth(currentDate.Year, quarter * 3));

            return lastDateOfQuarter;
        }
       

    }
}
