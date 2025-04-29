using Hangfire;
using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Interfaces;
using Res.Domain.Entities.ROAPayment;
using ResApp.Application.Com.Commands.RoTopUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ResApp.Application.ROA.SubscriptionPayment.Models;

namespace ResApp.Application.ROA.SubscriptionPayment.Command
{

    public class ROPaymentOnlineCommand : IRequest<Result<MemberSubPaymentRes>>
    {
        //  public SubscriptionFeeModel Model { get; set; } = new SubscriptionFeeModel();

        public int MemberId { get; set; }
        public List<PaymentTracking> Model { get; set; } = new List<PaymentTracking>();

    }

    public class ROPaymentOnlineCommandHandler : IRequestHandler<ROPaymentOnlineCommand, Result<MemberSubPaymentRes>>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        private readonly ICurrentUserService _currentUserService;
        private readonly IBroadcastHandler _broadcastHandler;
        private readonly IBackgroundJobClientV2 _backgroundJobClientV2;
        private readonly IMemLedgerService _memLedgerService;
        private readonly IMediator _mediator;
        public ROPaymentOnlineCommandHandler(IMemDbContext context, IPermissionHandler permissionHandler,
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
        public async Task<Result<MemberSubPaymentRes>> Handle(ROPaymentOnlineCommand request, CancellationToken cancellation)
        {

            using (var transaction = await _context.BeginTransactionAsync(cancellation))
            {
                try
                {
                    // var result = new MemberSubFeeListVm();
                    var result = new Result<MemberSubPaymentRes>
                    {
                        Data = new MemberSubPaymentRes()
                    };
                    string subscriptionModes = "";
                    decimal totalPayment = 0;
                    decimal currentbalance = 0;

                    // var lastPayemntNo = await _context.SubscriptionPaymentTemps.Select(s => s.PaymentNo).MaxAsync(cancellation);
                    var lastPayemntNo = await _context.ROASubscriptionPayments.Select(s => s.PaymentNo).MaxAsync(cancellation);
                    if (lastPayemntNo != "")
                    {
                        lastPayemntNo = (Convert.ToInt32(lastPayemntNo) + 1).ToString("00000000");
                    }
                    else
                    {
                        lastPayemntNo = "00000001";
                    }

                    var member = await _context.MemberRegistrationInfos.SingleOrDefaultAsync(q => q.Id == request.MemberId, cancellation);

                    if (member == null)
                    {
                        result.HasError = true;
                        result.Messages?.Add("Member not found");
                        return result;
                    }
                    if (!member.IsApproved)
                    {
                        result.HasError = true;
                        result.Messages?.Add("Member is not approved yet!!");
                        return result;
                    }
                    var lastSyncDate = await _context.RoSubscriptionDueTemps
                                            .OrderByDescending(q => q.SyncDate)
                                            .AsNoTracking()
                                            .FirstOrDefaultAsync();
                    var subPays = new List<ROASubscriptionPaymentDetail>();
                    var subcriptions = new List<RoSubscriptionDueTemp>();

                    var payemnt = new ROASubscriptionPayment
                    {
                        MemberId = request.MemberId,
                        MemberShipNo = member?.MemberShipNo ?? "",
                        // MemberPayment = totalAmount,
                        PaymentNo = lastPayemntNo,
                        // PaymentType= request.Model.PaymentType,
                        //  LateAmount = s.LateAmount,
                        //  LateFeePer = s.LateFePer,
                        ActualPaymentDate = DateTime.UtcNow,
                        // PaymentDate = s.PaymentDate,
                        //   SubscriptionFeesId = s.Id,
                        TotalAmount = request.Model.Sum(x => x.PaymentAmount + x.LateAmount),
                        IsPaid = true,
                        SubsStatus = "Paid",
                        //BankSlno = request.Model.BankId,
                        //PaymentTypeID = request.Model.PaymentTypeId,
                        //CardSlno = request.Model.CardId,

                    };
                    //  payemnt.TotalAmount = request.Model.Sum(s => s.PaymentAmount + s.LateAmount);
                    _context.ROASubscriptionPayments.Add(payemnt);

                    foreach (var s in request.Model)
                    {
                        var subDue = await _context.RoSubscriptionDueTemps
                                .AsNoTracking()
                                .FirstOrDefaultAsync(x => x.MemberId == request.MemberId
                                                    && x.SubscriptionMonth == s.SubscriptionMonth
                                                    && x.SubscriptionYear == s.SubscriptionMonth.Year);
                        if (subDue != null)
                        {
                            subDue.IsPaid = true;
                            _context.RoSubscriptionDueTemps.Update(subDue);
                        }
                        else
                        {
                            var obj = new RoSubscriptionDueTemp()
                            {
                                MemberShipNo = member!.MemberShipNo!,
                                MemberId = member.Id,
                                ActualPaymentDate = s.SubscriptionMonth,//  sub.StartDate,
                                SubscriptionFeeId = 1,// sub.Id,
                                SyncDate = lastSyncDate!.SyncDate,
                                GenerateDate = s.SubscriptionMonth,// new DateTime(request.SyncDate.Year, request.SyncDate.Month, 1),
                                LateFeePer = 0,// sub.LateFee ?? 0,
                                               //  SubscriptionYear = sub.SubscribedYear ?? "",
                                SubscriptionYear = s.SubscriptionMonth.Year,
                                PaymentAmount = s.PaymentAmount,//  m.SubscriptionFee?? sub.SubscriptionFee,
                                LateAmount = s.LateAmount,
                                SubscriptionName = "Monthly",
                                IsQBSync = false,
                                // IsPaid = false ,
                                IsPaid = true,
                                QBCusName = member.Name,
                                QBSyncDate = null,
                                GeneratedBy = "Online Payment",
                                SubscriptionMonth = s.SubscriptionMonth, //   sub.SubscribedMonth,

                            };
                            subcriptions.Add(obj);
                        }

                        //var checkExist = _context.ROASubscriptionPaymentDetail
                        //.AsNoTracking()
                        //.Any(x => x.MemberId == request.MemberId && x.SubscriptionMonth == s.SubscriptionMonth && x.SubscriptionYear == s.SubscriptionMonth.Year);

                        //  decimal totalAmount = 0;
                        //// if (pay == null)
                        //if (!checkExist)
                        //   {
                        //  pay = new SubscriptionPaymentTemp()
                        //var subDue = await _context.RoSubscriptionDueTemps.FirstOrDefaultAsync(x => x.MemberId == request.MemberId
                        //                        && x.SubscriptionMonth == s.SubscriptionMonth
                        //                        && x.SubscriptionYear == s.SubscriptionMonth.Year
                        //                        && x.IsPaid == false 
                        //);

                        // totalAmount += s.PaymentAmount;

                        var paymentDetail = new ROASubscriptionPaymentDetail()
                        {
                            MemberId = request.MemberId,
                            MemberShipNo = member?.MemberShipNo ?? "",
                            PaymentFees = s.PaymentAmount,
                            PaymentNo = lastPayemntNo,
                            LateFees = s.LateAmount, //s.LateAmount,
                                                     //  LateFeePer = s.LateFePer,
                            ActualPaymentDate = DateTime.Now,
                            PaymentFeesDate = s.SubscriptionMonth,
                            SubscriptionFeesId = 1, // s.Id,
                            SubscriptionName = "Montly", // s.SubscriptionName ?? "",
                            SubscriptionYear = s.SubscriptionMonth.Year, // s.PaymentYear,
                            IsPaid = true,
                            //SubsStatus = "Paid",
                            SubscriptionMonth = Convert.ToDateTime(s.SubscriptionMonth),
                            SubscriptionPaymentId = payemnt.Id,
                        };

                        subPays.Add(paymentDetail);


                        //  }






                    }
                    _context.ROASubscriptionPaymentDetail.AddRange(subPays);

                    if (subPays.Count > 0)
                    {
                        var lastMonth = request.Model.OrderByDescending(o => o.SubscriptionMonth).FirstOrDefault()?.SubscriptionMonth;


                        // Get the last day of that month
                        int lastDay = DateTime.DaysInMonth(lastMonth.GetValueOrDefault().Year, lastMonth.GetValueOrDefault().Month);



                        member!.PaidTill = new DateTime(lastMonth.GetValueOrDefault().Year, lastMonth.GetValueOrDefault().Month, lastDay);
                        _context.MemberRegistrationInfos.Update(member);

                        await _context.SaveChangesAsync(cancellation);



                        await transaction.CommitAsync(cancellation);
                        //}


                        //if (member != null)
                        //{

                        //    string message = "";
                        //    string subject = "";
                        // request.Model.Model.Where(q => q.IsChecked).OrderBy(o => o.SubscriptionMonth).ToList();

                        //    _context.RoMemberLedgers.AddRange(objList);
                        //if (member.Phone != null)
                        //{

                        //    message = "Dear CCCL Member, Tk. " + Math.Round(totalPayment, 2) + " has been deducted from your wallet, current balance is Tk. " + Math.Round(currentbalance, 2) + ", Thanks.";


                        //    _backgroundJobClientV2.Enqueue<IBroadcastHandler>(e => e.SendSms(member.Phone, message, "English", null, null));
                        //    message = "";
                        //}
                        //if (member.Email != null)
                        //{

                        //    message = "Dear CCCL Member, Tk. " + Math.Round(totalPayment, 2) + " has been deducted from your wallet, current balance is Tk. " + Math.Round(currentbalance, 2) + ", Thanks.";

                        //    subject = "Subscription Fee (Cadet College Club Ltd) ";
                        //    _backgroundJobClientV2.Enqueue<IBroadcastHandler>(e => e.SendEmail(member.Email, subject, message, null, null));
                        //    message = "";
                        //    subject = "";
                        //}


                        //var memberSubFee = new MemberSubPaymentRes()
                        //{
                        //    PaymentNo = payemnt.PaymentNo,
                        //};
                        result.Data.PaymentNo = payemnt.PaymentNo;

                        result.HasError = false;
                        result.Messages?.Add("Payment has been done successfully");
                        return result;
                    }

                    result.HasError = true;
                    result.Messages?.Add("Something went wrong!!!");
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
