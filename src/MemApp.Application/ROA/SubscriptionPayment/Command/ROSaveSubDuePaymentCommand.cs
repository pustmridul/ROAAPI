using Hangfire;
using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Interfaces;
using MemApp.Application.Mem.Subscription.Model;
using MemApp.Application.Mem.TopUps.Command;
using MemApp.Domain.Entities.mem;
using MemApp.Domain.Entities.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MemApp.Application.Extensions;
using Res.Domain.Entities.ROAPayment;
using Res.Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using ResApp.Application.Com.Commands.RoTopUp;
using ResApp.Application.ROA.SubscriptionPayment.Models;

namespace ResApp.Application.ROA.SubscriptionPayment.Command
{

    public class ROSaveSubDuePaymentCommand : IRequest<ListResult<MemberSubPaymentRes>>
    {
        //  public SubscriptionFeeModel Model { get; set; } = new SubscriptionFeeModel();
        public SubscriptionSubDuePayReq Model { get; set; } = new SubscriptionSubDuePayReq();

    }

    public class ROSaveSubDuePaymentCommandHandler : IRequestHandler<ROSaveSubDuePaymentCommand, ListResult<MemberSubPaymentRes>>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        private readonly ICurrentUserService _currentUserService;
        private readonly IBroadcastHandler _broadcastHandler;
        private readonly IBackgroundJobClientV2 _backgroundJobClientV2;
        private readonly IMemLedgerService _memLedgerService;
        private readonly IMediator _mediator;
        public ROSaveSubDuePaymentCommandHandler(IMemDbContext context, IPermissionHandler permissionHandler,
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
        public async Task<ListResult<MemberSubPaymentRes>> Handle(ROSaveSubDuePaymentCommand request, CancellationToken cancellation)
        {



            using (var transaction = await _context.BeginTransactionAsync(cancellation))
            {
                try
                {
                    // var result = new MemberSubFeeListVm();
                    var result = new ListResult<MemberSubPaymentRes>
                    {
                        Data = new List<MemberSubPaymentRes>()
                    };
                    string subscriptionModes = "";
                    decimal totalPayment = 0;
                    decimal currentbalance = 0;
                    var checkedList = request.Model.Model.Where(q => q.IsChecked).OrderBy(o => o.SubscriptionMonth).ToList();
                    var uncheckedList = request.Model.Model.Where(q => !q.IsChecked).OrderBy(o => o.SubscriptionMonth).ToList();

                    foreach (var item in checkedList)
                    {
                        if (item.SubscriptionMonth > uncheckedList.FirstOrDefault()?.SubscriptionMonth)
                        {
                            result.HasError = true;
                            result.Messages?.Add("Please previous due payment first !");
                            return result;
                        }
                    }
                    // need to fix

                    if (checkedList.Count == 0)
                    {
                        result.HasError = true;
                        result.Messages?.Add("Someting went wrong!");
                        return result;
                    }
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

                    var member = await _context.MemberRegistrationInfos.SingleOrDefaultAsync(q => q.Id == request.Model.MemberId, cancellation);

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

                    var subPays = new List<ROASubscriptionPaymentDetail>();

                    var payemnt = new ROASubscriptionPayment
                    {
                        MemberId = request.Model.MemberId,
                        MemberShipNo = member?.MemberShipNo ?? "",
                        // MemberPayment = totalAmount,
                        PaymentNo = lastPayemntNo,
                        // PaymentType= request.Model.PaymentType,
                        //  LateAmount = s.LateAmount,
                        //  LateFeePer = s.LateFePer,
                        ActualPaymentDate = DateTime.Now,
                        // PaymentDate = s.PaymentDate,
                        //   SubscriptionFeesId = s.Id,

                        IsPaid = true,
                        SubsStatus = "Paid",
                        //BankSlno = request.Model.BankId,
                        //PaymentTypeID = request.Model.PaymentTypeId,
                        //CardSlno = request.Model.CardId,

                    };
                    payemnt.TotalAmount = checkedList.Sum(s => s.PaymentAmount);
                    _context.ROASubscriptionPayments.Add(payemnt);

                    //var memLedger = new MemLedger()
                    //{
                    //    ReferenceId = lastPayemntNo,
                    //    Amount = payemnt.TotalAmount, // Math.Round((-1) * (pay.PaymentAmount + pay.LateAmount), 2),
                    //    Dates = DateTime.Now,
                    //    PrvCusID = request.Model.MemberId.ToString(),
                    //    TOPUPID = "",
                    //    UpdateBy = _currentUserService.Username,
                    //    UpdateDate = DateTime.Now,
                    //    DatesTime = DateTime.Now,
                    //  //  Notes = "Subscription fee year : " + pay.SubscriptionYear + " Quater : " + pay.SubscriptionName,
                    //    Notes = "Subscription fee Details : " + checkedList.Count+ " Months, "+ checkedList.FirstOrDefault()!.SubscriptionMonth.ToString("MMMM, yyyyy") + " To  " + 
                    //                                       checkedList.LastOrDefault()!.SubscriptionMonth.ToString("MMMM, yyyyy"),
                    //    PayType = "",
                    // //   Description = "Subscription fee year : " + pay.SubscriptionYear + " Quater : " + pay.SubscriptionName,
                    //    Description= "Subscription fee Details : " + checkedList.Count + " Months, " + checkedList.FirstOrDefault()!.SubscriptionMonth.ToString("MMMM, yyyyy") + " To  " +
                    //                                       checkedList.LastOrDefault()!.SubscriptionMonth.ToString("MMMM, yyyyy"),
                    //    TransactionType = "SUBSCRIPTION",
                    //    TransactionFrom = _currentUserService.AppId,

                    //};

                    //_context.MemLedgers.Add(memLedger);

                    foreach (var s in request.Model.Model.Where(q => q.IsChecked))
                    {


                        var checkExist = _context.ROASubscriptionPaymentDetail
                        .AsNoTracking()
                        .Any(x => x.MemberId == request.Model.MemberId && x.SubscriptionMonth == s.SubscriptionMonth && x.SubscriptionYear == s.SubscriptionYear);

                        decimal totalAmount = 0;
                        // if (pay == null)
                        if (!checkExist)
                        {
                            //  pay = new SubscriptionPaymentTemp()
                            var subDue = await _context.RoSubscriptionDueTemps.FirstOrDefaultAsync(x => x.MemberId == request.Model.MemberId
                                                    && x.SubscriptionMonth == s.SubscriptionMonth
                                                    && x.SubscriptionYear == s.SubscriptionYear
                                                    && x.IsPaid == false && x.Id == s.Id
                            );
                            if (subDue != null)
                            {
                                totalAmount += s.PaymentAmount;

                                var paymentDetail = new ROASubscriptionPaymentDetail()
                                {
                                    MemberId = request.Model.MemberId,
                                    MemberShipNo = member?.MemberShipNo ?? "",
                                    PaymentFees = member!.SubscriptionFee.GetValueOrDefault(),
                                    PaymentNo = lastPayemntNo,
                                    LateFees = subDue.LateFeePer, //s.LateAmount,
                                    //  LateFeePer = s.LateFePer,
                                    ActualPaymentDate = DateTime.Now,
                                    PaymentFeesDate = subDue.ActualPaymentDate,
                                    SubscriptionFeesId = 1, // s.Id,
                                    SubscriptionName = "Montly", // s.SubscriptionName ?? "",
                                    SubscriptionYear = s.SubscriptionYear, // s.PaymentYear,
                                    IsPaid = true,
                                    //SubsStatus = "Paid",
                                    SubscriptionMonth = Convert.ToDateTime(subDue.SubscriptionMonth),
                                    SubscriptionPaymentId = payemnt.Id
                                };

                                subPays.Add(paymentDetail);

                                subDue.IsPaid = true;
                                _context.RoSubscriptionDueTemps.Update(subDue);
                            }





                        }
                    }
                    _context.ROASubscriptionPaymentDetail.AddRange(subPays);

                    if (subPays.Count > 0)
                    {
                        if (request.Model.topup is not null)
                        {
                            await _mediator.Send(new CreateRoTopUpCommand()
                            {
                                Model = request.Model.topup,
                                MonthCount = checkedList.Count,
                                StartingMonth = checkedList.FirstOrDefault()!.SubscriptionMonth,
                                EndingMonth = checkedList.LastOrDefault()!.SubscriptionMonth
                            });
                        }
                        var lastMonth = request.Model.Model.OrderByDescending(o => o.SubscriptionMonth).FirstOrDefault(q => q.IsChecked)?.SubscriptionMonth;


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


                        var memberSubFee = new MemberSubPaymentRes()
                        {
                            PaymentNo = payemnt.PaymentNo,
                        };
                        result.Data.Add(memberSubFee);

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
