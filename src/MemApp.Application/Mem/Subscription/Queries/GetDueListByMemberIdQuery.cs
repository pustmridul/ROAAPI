using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Subscription.Model;
using MemApp.Domain.Entities.subscription;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Subscription.Queries
{

    public class GetDueListByMemberIdQuery : IRequest<MemberSubFeeListVm>
    {
        public int Id { get; set; }
    }

    public class GetDueListByMemberIdQueryHandler : IRequestHandler<GetDueListByMemberIdQuery, MemberSubFeeListVm>
    {
        private readonly IMemDbContext _context;
        public GetDueListByMemberIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<MemberSubFeeListVm> Handle(GetDueListByMemberIdQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var result = new MemberSubFeeListVm();

                var memberObj = await _context.RegisterMembers
                   .Include(i => i.MemberTypes)
                   .Include(i => i.MemberActiveStatus)
                   .AsNoTracking()
                   .SingleOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
                //.Select(s=> new {s.Id, s.PaidTill, s.Phone, s.Email, s.MembershipNo, s.MemberTypeId})


                var subChargeList = new List<SubscriptionFees>();
                if (memberObj != null)
                {
                    var memberType = await _context.MemberTypes
                        .SingleOrDefaultAsync(q => q.Id == memberObj.MemberTypeId, cancellationToken);

                    if (memberType != null)
                    {
                        if (!memberType.IsSubscribed)
                        {
                            result.HasError = false;
                            result.Messages?.Add("This membership type no needed subscription.");
                            return result;
                        }
                    }
                    if (memberObj.PaidTill != null)
                    {
                        subChargeList = await _context.SubscriptionFees
                            .Include(i => i.SubscriptionMode)
                            .Where(q => q.IsActive && q.StartDate > memberObj.PaidTill && q.StartDate.Date <= GetCurrentQuater().Date)
                            .OrderBy(c => c.StartDate)
                            .AsNoTracking()
                            .ToListAsync(cancellationToken);
                    }
                    else
                    {
                        subChargeList = await _context.SubscriptionFees
                            .Include(i => i.SubscriptionMode)
                            .Where(q => q.IsActive && q.StartDate.Date < DateTime.Now.Date && q.StartDate.Date <= GetCurrentQuater().Date)
                            .OrderBy(c => c.StartDate)
                            .AsNoTracking()
                            .ToListAsync(cancellationToken);
                    }


                    var subsPayments = await _context.SubscriptionPaymentTemps
                        .Where(q => q.RegisterMemberId == memberObj.Id)
                        .AsNoTracking()
                        .ToListAsync(cancellationToken);


                    var memberSubFees = new List<MemberSubFee>();
                    foreach (var sub in subChargeList)
                    {
                        var has = subsPayments.Where(q => q.SubscriptionFeesId == sub.Id).Any();
                        if (!has)
                        {
                            var obj = new MemberSubFee()
                            {
                                Id = sub.Id,
                                PaymentDate = sub.StartDate,
                                PaymentYear = sub.SubscribedYear ?? "",

                                PaymentAmount = (memberObj.MemberActiveStatus?.Name == "Abroad" || memberObj.MemberActiveStatus?.Name == "Absentee") ? ((sub.AbroadFee ?? 0) / 100) * sub.SubscriptionFee : sub.SubscriptionFee,
                                LateAmount = (memberObj.MemberActiveStatus?.Name == "Abroad" || memberObj.MemberActiveStatus?.Name == "Absentee") ?
                               (sub.SubscriptionFee * ((sub.AbroadFee ?? 0) / 100) * LateCal(sub.StartDate, (sub.LateFee ?? 0))) / 100 : (LateCal(sub.StartDate, (sub.LateFee ?? 0)) / 100) * sub.SubscriptionFee,

                                SubscriptionName = sub.SubscriptionMode.Name,

                                AbroadFeeAmount = sub.SubscriptionFee * ((sub.AbroadFee ?? 0) / 100),
                                AbroadFeePer = sub.AbroadFee ?? 0
                            };
                            memberSubFees.Add(obj);
                        }

                    }
                    result.HasError = false;
                    result.DataList = memberSubFees;
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
        private DateTime GetCurrentQuater()
        {
            DateTime currentDate = DateTime.Now;
            int quarter = (currentDate.Month - 1) / 3 + 1;
            DateTime lastDateOfQuarter = new DateTime(currentDate.Year, quarter * 3, DateTime.DaysInMonth(currentDate.Year, quarter * 3));

            return lastDateOfQuarter;
        }
        private decimal LateCal(DateTime startDate, decimal lateFeePer)
        {
            int monthsApart = (DateTime.Now.Year - startDate.Year) * 12 + DateTime.Now.Month - startDate.Month;
            return (monthsApart + 1) * lateFeePer;
        }
    }

}
