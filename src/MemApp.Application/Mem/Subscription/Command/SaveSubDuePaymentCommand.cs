using AutoMapper.Execution;
using Hangfire;
using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.MemberStatuss.Queries;
using MemApp.Application.Mem.Subscription.Model;
using MemApp.Application.Mem.TopUps.Command;
using MemApp.Application.Mem.TopUps.Models;
using MemApp.Domain.Entities.mem;
using MemApp.Domain.Entities.Payment;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Subscription.Command
{
    public class SaveSubDuePaymentCommand : IRequest<MemberSubFeeListVm>
    {
      public SubscriptionFeeModel Model { get; set; } = new SubscriptionFeeModel();
    }

    public class SaveSubDuePaymentCommandHandler : IRequestHandler<SaveSubDuePaymentCommand, MemberSubFeeListVm>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        private readonly ICurrentUserService _currentUserService;
        private readonly IBroadcastHandler _broadcastHandler;
        private readonly IBackgroundJobClientV2 _backgroundJobClientV2;
        private readonly IMemLedgerService _memLedgerService;
        private readonly IMediator _mediator;
        public SaveSubDuePaymentCommandHandler(IMemDbContext context, IPermissionHandler permissionHandler,
            ICurrentUserService currentUserService, IBroadcastHandler broadcastHandler, IBackgroundJobClientV2 backgroundJobClientV2,
            IMemLedgerService memLedgerService, IMediator mediator)
        {
            _context = context;
            _permissionHandler = permissionHandler;
            _currentUserService = currentUserService;
            _broadcastHandler = broadcastHandler;
            _backgroundJobClientV2 = backgroundJobClientV2;
            _memLedgerService = memLedgerService;
            _mediator = mediator;
        }
        public async Task<MemberSubFeeListVm> Handle(SaveSubDuePaymentCommand request, CancellationToken cancellation)
        {

            if (request.Model.topup is not null)
            {
                await _mediator.Send(new CreateTopUpCommand() { Model = request.Model.topup });
            }

            using (var transaction = await _context.BeginTransactionAsync(cancellation))
            {
                try
                {
                    var result = new MemberSubFeeListVm();
                    string subscriptionModes = "";
                    decimal totalPayment = 0;
                    decimal currentbalance=0;
                    var checkedList = request.Model.Model.Where(q => q.IsChecked).OrderBy(o => o.PaymentDate).ToList();
                    var uncheckedList = request.Model.Model.Where(q => !q.IsChecked).OrderBy(o => o.PaymentDate).ToList();

                    foreach (var item in checkedList)
                    {
                        if (item.PaymentDate > uncheckedList.FirstOrDefault()?.PaymentDate)
                        {
                            result.HasError = true;
                            result.Messages?.Add("Please previous due payment first !");
                            return result;
                        }
                    }
                    // need to fix


                    var lastPayemntNo = await _context.SubscriptionPaymentTemps.Select(s => s.PaymentNo).MaxAsync(cancellation);
                    if (lastPayemntNo != "")
                    {
                        lastPayemntNo = (Convert.ToInt32(lastPayemntNo) + 1).ToString("0000000");
                    }
                    else
                    {
                        lastPayemntNo = "0000001";
                    }
                    var member = await _context.RegisterMembers.SingleOrDefaultAsync(q => q.Id == request.Model.MemberId, cancellation);

                    if (member == null)
                    {
                        result.HasError = true;
                        result.Messages?.Add("Member not found");
                        return result;
                    }

                    var subPays = new List<SubscriptionPaymentTemp>();
                    List<MemLedger> objList = new List<MemLedger>();
                    foreach (var s in request.Model.Model.Where(q => q.IsChecked))
                    {
                        var pay = await _context.SubscriptionPaymentTemps
                            .SingleOrDefaultAsync(q => q.RegisterMemberId == request.Model.MemberId && q.SubscriptionFeesId == s.Id);

                        if (pay == null)
                        {
                            pay = new SubscriptionPaymentTemp()
                            {
                                RegisterMemberId = request.Model.MemberId,
                                MemberShipNo = member?.MembershipNo ?? "",
                                PaymentAmount = s.PaymentAmount,
                                PaymentNo = lastPayemntNo,
                                LateAmount = s.LateAmount,
                                LateFeePer = s.LateFePer,
                                ActualPaymentDate = DateTime.Now,
                                PaymentDate = s.PaymentDate,
                                SubscriptionFeesId = s.Id,
                                SubscriptionName = s.SubscriptionName ?? "",
                                SubscriptionYear = s.PaymentYear,
                                IsPaid = true,
                                SubsStatus = "Paid",
                                AbroadFeeAmount = s.AbroadFeeAmount,
                                AbroadFeePer = s.AbroadFeePer,
                            };
                            subPays.Add(pay);

                            pay.IsPaid = true;
                            pay.SubsStatus = "Paid";

                            var memLedger = new MemLedger()
                            {
                                ReferenceId = lastPayemntNo,
                                Amount = Math.Round((-1) * (pay.PaymentAmount + pay.LateAmount), 2),
                                Dates = DateTime.Now,
                                PrvCusID = request.Model.MemberId.ToString(),
                                TOPUPID = "",
                                UpdateBy = _currentUserService.Username,
                                UpdateDate = DateTime.Now,
                                DatesTime = DateTime.Now,
                                Notes = "Subscription fee year : " + pay.SubscriptionYear + " Quater : " + pay.SubscriptionName,
                                PayType = "",
                                Description = "Subscription fee year : " + pay.SubscriptionYear + " Quater : " + pay.SubscriptionName,
                                TransactionType = "SUBSCRIPTION",
                                TransactionFrom = _currentUserService.AppId,

                            };
                            objList.Add(memLedger);
                            subscriptionModes = subscriptionModes + pay.SubscriptionYear + ":" + pay.SubscriptionName + " ";
                            totalPayment = totalPayment + pay.PaymentAmount;

                            var memberSubFee = new MemberSubFee()
                            {
                                PaymentNo = pay.PaymentNo,
                            };
                            result.DataList.Add(memberSubFee);
                        }
                    }
                    if (member != null)
                    {
                        if (member.PrvCusID != null)
                        {
                             currentbalance = await _memLedgerService.GetCurrentBalance(member.PrvCusID);

                            if (currentbalance < totalPayment)
                            {
                                if(request.Model.topup is not null)
                                {
                                    if (request.Model.topup.TopUpDetails.Sum(s => s.Amount) < totalPayment)
                                    {
                                        result.HasError = true;
                                        result.Messages?.Add("Top-up balance is less then payment amount");
                                        return result;
                                    }
                                }
                            }

                          
                        }

                    }


                    _context.SubscriptionPaymentTemps.AddRange(subPays);
                    if(await _context.SaveChangesAsync(cancellation) > 0)
                    {
                        if (member != null)
                        {
                           
                            string message = "";
                            string subject = "";
                            member.PaidTill = request.Model.Model.OrderByDescending(o => o.PaymentDate).FirstOrDefault(q => q.IsChecked)?.PaymentDate;
                            _context.MemLedgers.AddRange(objList);
                            if (member.Phone != null)
                            {

                              message = "Dear CCCL Member, Tk. " + Math.Round(totalPayment, 2) + " has been deducted from your wallet, current balance is Tk. " + Math.Round(currentbalance, 2) + ", Thanks.";


                                _backgroundJobClientV2.Enqueue<IBroadcastHandler>(e => e.SendSms(member.Phone, message, "English", null, null));
                                message = "";
                            }
                            if (member.Email != null)
                            {

                                message = "Dear CCCL Member, Tk. " + Math.Round(totalPayment, 2) + " has been deducted from your wallet, current balance is Tk. " + Math.Round(currentbalance, 2) + ", Thanks.";

                                subject = "Subscription Fee (Cadet College Club Ltd) ";
                                _backgroundJobClientV2.Enqueue<IBroadcastHandler>(e => e.SendEmail(member.Email, subject, message, null, null));
                                message = "";
                                subject = "";
                            }
                        }
                        await _context.SaveChangesAsync(cancellation);
                    }
                    
                   
                    await transaction.CommitAsync(cancellation);
                    return result;
                }
                catch (Exception ex) {
                    throw new Exception(ex.Message);
                }
            }
            
        }

    }
}
