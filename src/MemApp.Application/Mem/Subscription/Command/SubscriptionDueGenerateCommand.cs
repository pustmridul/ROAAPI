using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Models;
using MemApp.Domain.Entities.Payment;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;

namespace MemApp.Application.Mem.Subscription.Command
{
    public class SubscriptionDueGenerateCommand : IRequest<Result>
    {
        public DateTime SyncDate { get; set; } = DateTime.Now;
    }

    public class SubscriptionDueGenerateCommandHandler : IRequestHandler<SubscriptionDueGenerateCommand, Result>
    {
        private readonly IMemDbContext _context;
        public SubscriptionDueGenerateCommandHandler(IMemDbContext context)
        {
            _context = context;
        }
        public async Task<Result> Handle(SubscriptionDueGenerateCommand request, CancellationToken cancellationToken)
        {
            var result = new Result();
            var dataList = new List<SubscriptionDueTemp>();
            DateTime startDate =  new DateTime(2024, 6, 1); 
            var memberList = await _context.RegisterMembers
                   .Include(i => i.MemberTypes)
                   .Include(i => i.MemberActiveStatus)
                   .Where(q => q.IsActive && q.IsMasterMember == true && q.MemberStatusId!=16 && q.PaidTill.Value.Date < request.SyncDate.Date && q.MemberTypes.IsSubscribed)
                   .AsNoTracking()
                   .ToListAsync(cancellationToken);

            var subChargeList = await _context.SubscriptionFees
                             .Include(i => i.SubscriptionMode)
                             .Where(q => q.IsActive &&  q.StartDate.Date <= request.SyncDate)
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
                       var dueSubscription = subChargeList.Where(q => q.StartDate>m.PaidTill ).ToList(); // Checking any month 

                    if (dueSubscription.Any())
                    {
                        foreach (var sub in dueSubscription)
                        {
                            if(sub.StartDate.Date== request.SyncDate.Date)
                            {

                                if (request.SyncDate.Month == 1 || request.SyncDate.Month == 4 || request.SyncDate.Month == 7 || request.SyncDate.Month == 10)
                                {

                                    var obj = new SubscriptionDueTemp()
                                    {
                                        MemberShipNo = m.MembershipNo,
                                        MemberId = m.Id,
                                        ActualPaymentDate = sub.StartDate,
                                        SubscriptionFeesId = sub.Id,
                                        SyncDate = request.SyncDate,
                                        GenerateDate = new DateTime(request.SyncDate.Year, request.SyncDate.Month, 1),
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
                                        MemberShipNo = m.MembershipNo,
                                        ActualPaymentDate = sub.StartDate,
                                        SubscriptionFeesId = sub.Id,
                                        MemberId = m.Id,
                                        SyncDate = request.SyncDate,
                                        GenerateDate = new DateTime(request.SyncDate.Year, request.SyncDate.Month, 1),
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
            result.HasError = false;
            result.Messages.Add(" Date : "+ request.SyncDate + "Sync success");
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