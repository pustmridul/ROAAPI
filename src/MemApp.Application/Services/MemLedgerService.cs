using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Subscription.Model;
using MemApp.Application.Models;
using MemApp.Domain.Entities.mem;
using MemApp.Domain.Entities.Payment;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Services
{
    public class MemLedgerService : IMemLedgerService
    {
        private readonly IMemDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDapperContext _dapperContext;

        public MemLedgerService(IMemDbContext context, ICurrentUserService currentUserService, IDapperContext dapperContext)
        {
            _context = context;
            _currentUserService = currentUserService;
            _dapperContext = dapperContext;
        }
        public async Task<bool> CreateMemLedger(MemLedgerVm model)
        {

            try
            {
                var obj = await _context.MemLedgers.SingleOrDefaultAsync(q => q.CustomerLedgerID == model.CustomerLedgerID);
                if (obj == null)
                {
                    var member = await _context.RegisterMembers.Select(s => new { s.Id, s.PrvCusID, s.IsMasterMember, s.MemberId })
                        .SingleOrDefaultAsync(q => q.Id == Convert.ToInt32(model.PrvCusID));

                    obj = new MemLedger();
                    if (member != null)
                    {
                        if (member.IsMasterMember == true)
                        {
                            if (member.PrvCusID == "0")
                            {
                                return false;
                            }
                            obj.PrvCusID = member.PrvCusID ?? "";
                        }
                        else
                        {
                            var memObj = await _context.RegisterMembers.Select(s => new { s.Id, s.PrvCusID, s.IsMasterMember, s.MemberId })
                                         .SingleOrDefaultAsync(q => q.Id == member.MemberId);

                            if (memObj?.PrvCusID == "0")
                            {
                                return false;
                            }

                            obj.PrvCusID = memObj?.PrvCusID ?? "";
                        }

                    }
                    obj.TOPUPID = model.TOPUPID;

                    _context.MemLedgers.Add(obj);
                }

                obj.ReferenceId = model.ReferenceId;
                obj.UpdateBy = _currentUserService.Username;
                obj.UpdateDate = DateTime.Now;
                obj.DatesTime = DateTime.Now;

                obj.Amount = Math.Round(model.Amount ?? 0, 2);
                obj.Notes = model.Notes;
                obj.RefundId = model.RefundId;
                obj.ServiceChargeAmount = model.ServiceChargeAmount;
                obj.ChequeCardNo = model.ChequeCardNo;
                obj.BankCreditCardName = model.BankCreditCardName;
                obj.PayType = model.PayType;
                obj.Description = model.Description;
                obj.TransactionFrom = model.TransactionFrom;
                obj.TransactionType = model.TransactionType;

                obj.Dates = model.Dates;


                if (await _context.SaveAsync() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }



        public async Task<bool> CreateBulkMemLedger(List<MemLedgerVm> models)
        {
            List<MemLedger> objList = new List<MemLedger>();

            var member = await _context.RegisterMembers.Select(s => new
            {
                s.Id,
                s.PrvCusID
            }).SingleOrDefaultAsync(q => q.Id == Convert.ToInt32(models.FirstOrDefault().PrvCusID));



            foreach (var m in models)
            {

                var obj = new MemLedger();
                obj.PrvCusID = member?.PrvCusID ?? "";
                obj.ReferenceId = m.ReferenceId;
                obj.TOPUPID = m.TOPUPID;
                obj.UpdateBy = _currentUserService.Username;
                obj.UpdateDate = DateTime.Now;
                obj.DatesTime = DateTime.Now;

                obj.Amount = Math.Round(m.Amount ?? 0, 2);
                obj.Notes = m.Notes;
                obj.RefundId = m.RefundId;
                obj.ServiceChargeAmount = m.ServiceChargeAmount;
                obj.ChequeCardNo = m.ChequeCardNo;
                obj.BankCreditCardName = m.BankCreditCardName;
                obj.PayType = m.PayType;
                obj.Description = m.Description;
                obj.Dates = m.Dates;
                obj.TransactionType = m.TransactionType;
                obj.TransactionFrom = m.TransactionFrom;

                objList.Add(obj);
            }

            _context.MemLedgers.AddRange(objList);
            if (await _context.SaveAsync() > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<decimal> GetCurrentBalance(string memberId)
        {
            decimal currentbalance = 0;
            var memObj = await _context.RegisterMembers.FirstOrDefaultAsync(q => q.PrvCusID == memberId, new CancellationToken());
            if (memObj?.IsMasterMember == true)
            {
                currentbalance = await _context.MemLedgers.Where(q => q.PrvCusID == memberId).SumAsync(s => s.Amount) ?? 0;
            }
            else
            {
                var spouseObj = await _context.RegisterMembers.FirstOrDefaultAsync(q => q.Id == Convert.ToInt32(memberId), new CancellationToken());
                currentbalance = await _context.MemLedgers.Where(q => q.PrvCusID == spouseObj.MemberId.ToString()).SumAsync(s => s.Amount) ?? 0;
            }
            return currentbalance;
        }

        public async Task<bool> AutoSubscriptionPayment(string SubscriptionYear, string QuaterNo)
        {
            var memberList = await _context.RegisterMembers.Include(i => i.MemberTypes)
               .Where(q => q.MemberTypes.IsSubscribed
               && q.IsActive
               && q.IsMasterMember == true
               && q.PaidTill < DateTime.Now
               )
               .Select(s =>
           new SubscriptionMember
           {
               PaidTill = s.PaidTill,
               MemberShipNo = s.MembershipNo ?? "",
               MemberId = s.Id
           }).ToListAsync();


            var subsDueFeeList = await _context.SubscriptionFees
            .Where(
                q => q.SubscribedYear == SubscriptionYear
                && q.Title == QuaterNo
                )
                .ToListAsync();
            List<SubscriptionPaymentTemp> subscriptionPaymentTempList = new List<SubscriptionPaymentTemp>();
            foreach (var sub in subsDueFeeList)
            {
                foreach (var m in memberList)
                {
                    var subPayment = await _context.SubscriptionPaymentTemps.SingleOrDefaultAsync(q =>
                    q.RegisterMemberId == m.MemberId
                    && q.SubscriptionFeesId == sub.Id
                    );

                    if (subPayment == null)
                    {
                        subPayment = new SubscriptionPaymentTemp()
                        {
                            RegisterMemberId = m.MemberId,
                            SubscriptionFeesId = sub.Id,
                            SubsStatus = "Due",
                            PaymentAmount = sub.SubscriptionFee,
                            PaymentDate = DateTime.Now,
                            IsPaid = false,
                            MemberShipNo = m.MemberShipNo
                        };
                        subscriptionPaymentTempList.Add(subPayment);
                    }
                }
            }
            _context.SubscriptionPaymentTemps.AddRange(subscriptionPaymentTempList);

            await _context.SaveAsync();
            return true;
        }




    }
}
