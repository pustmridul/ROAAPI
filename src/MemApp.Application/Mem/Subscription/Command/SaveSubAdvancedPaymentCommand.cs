using Hangfire;
using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Subscription.Model;
using MemApp.Application.Mem.TopUps.Command;
using MemApp.Domain.Entities.mem;
using MemApp.Domain.Entities.Payment;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Subscription.Command
{
    public class SaveSubAdvancedPaymentCommand : IRequest<MemberSubFeeListVm>
    {
        public SubscriptionFeeModel Model { get; set; } = new SubscriptionFeeModel();
    }

    public class SaveSubAdvancedPaymentCommandHandler : IRequestHandler<SaveSubAdvancedPaymentCommand, MemberSubFeeListVm>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        private readonly ICurrentUserService _currentUserService;
        private readonly IBroadcastHandler _broadcastHandler;
        private readonly IBackgroundJobClientV2 _backgroundJobClientV2;
        private readonly IMemLedgerService _memLedgerService;
        private readonly IMediator _mediator;
        public SaveSubAdvancedPaymentCommandHandler(
            IMemDbContext context,
            IPermissionHandler permissionHandler,
            ICurrentUserService currentUserService,
            IBroadcastHandler broadcastHandler,
            IBackgroundJobClientV2 backgroundJobClientV2,
            IMemLedgerService memLedgerService,
            IMediator mediator)
        {
            _context = context;
            _permissionHandler = permissionHandler;
            _currentUserService = currentUserService;
            _broadcastHandler = broadcastHandler;
            _backgroundJobClientV2 = backgroundJobClientV2;
            _memLedgerService = memLedgerService;
            _mediator = mediator;
        }
        public async Task<MemberSubFeeListVm> Handle(SaveSubAdvancedPaymentCommand request, CancellationToken cancellation)
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
                    List<MemLedger> objList = new List<MemLedger>();
                    string subscriptionModes = "";
                    decimal totalPayment = 0;
                    decimal currentbalance = 0;

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
                    if (await _permissionHandler.IsTempMember())
                    {
                        result.HasError = true;
                        result.Messages?.Add("You have no permission to access, please contact with authority.");
                        return result;
                    }


                    var member = await _context.RegisterMembers.SingleOrDefaultAsync(s => s.Id == request.Model.MemberId, cancellation);

                    if (member == null)
                    {

                        result.HasError = true;
                        result.Messages?.Add("Member is not found");
                        return result;
                    }
                   

                    var lastPayemntNo = await _context.SubscriptionPaymentTemps.Select(s => s.PaymentNo).MaxAsync(cancellation);
                    if (lastPayemntNo != "")
                    {
                        lastPayemntNo = (Convert.ToInt32(lastPayemntNo) + 1).ToString("0000000");
                    }
                    else
                    {
                        lastPayemntNo = "0000001";
                    }


                    foreach (var s in request.Model.Model.OrderBy(o => o.PaymentDate).Where(q => q.IsChecked))
                    {
                        var pay = await _context.SubscriptionPaymentTemps
                            .SingleOrDefaultAsync(q => q.SubscriptionFeesId == s.Id
                            && q.RegisterMemberId == request.Model.MemberId, cancellation);

                        if (pay == null)
                        {
                            pay = new SubscriptionPaymentTemp();
                            _context.SubscriptionPaymentTemps.Add(pay);
                        }
                        pay.ActualPaymentDate = DateTime.Now;
                        pay.IsPaid = true;
                        pay.PaymentNo = lastPayemntNo;
                        pay.SubsStatus = "Paid";
                        pay.SubscriptionFeesId = s.Id;
                        pay.PaymentAmount = s.PaymentAmount;
                        pay.IsPaid = true;
                        pay.PaymentDate = s.PaymentDate;
                        pay.MemberShipNo = member?.MembershipNo ?? "";
                        pay.RegisterMemberId = member?.Id ?? 0;
                        pay.SubscriptionYear = s.PaymentYear;
                        pay.SubscriptionName = s.SubscriptionName ?? "";


                        var memLedger = new MemLedger()
                        {
                            Amount = (-1) * pay.PaymentAmount,
                            Dates = DateTime.Now,
                            PrvCusID = request.Model.MemberId.ToString(),
                            TOPUPID = "",
                            UpdateBy = _currentUserService.Username,
                            UpdateDate = DateTime.Now,
                            DatesTime = DateTime.Now,
                            Notes = "Subscription fee year : " + pay.SubscriptionYear + " Quater : " + pay.SubscriptionName,
                            PayType = "",
                            Description = "Subscription fee year : " + pay.SubscriptionYear + " Quater : " + pay.SubscriptionName,
                            TransactionFrom = _currentUserService.AppId,
                            TransactionType = "SUBSCRIPTION"

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


                    if (member != null)
                    {
                        if (member.PrvCusID != null)
                        {
                            currentbalance = await _memLedgerService.GetCurrentBalance(member.PrvCusID);

                            if (currentbalance < totalPayment)
                            {
                                if (request.Model.topup is not null)
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

                    if (await _context.SaveChangesAsync(cancellation) > 0)
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }


        }

    }
}
