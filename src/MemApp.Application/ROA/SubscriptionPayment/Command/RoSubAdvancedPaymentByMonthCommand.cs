using Hangfire;
using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Interfaces;
using Res.Domain.Entities.ROAPayment;
using Res.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ResApp.Application.ROA.SubscriptionPayment.Models;

namespace ResApp.Application.ROA.SubscriptionPayment.Command
{

    public class RoSubAdvancedPaymentByMonthCommand : IRequest<ListResult<MemberSubPaymentRes>>
    {
        //  public SubscriptionSubDuePayReq Model { get; set; } = new SubscriptionSubDuePayReq();

        public int MemberId { get; set; }
        public DateTime Month { get; set; }
        public string PaymentType { get; set; }
        public int? PaymentTypeId { get; set; }
        public int? BankId { get; set; }
        public int? CardId { get; set; }

    }
    public class RoSubAdvancedPaymentByMonthHandlerCommandHandler : IRequestHandler<RoSubAdvancedPaymentByMonthCommand, ListResult<MemberSubPaymentRes>>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        private readonly ICurrentUserService _currentUserService;
        private readonly IBroadcastHandler _broadcastHandler;
        private readonly IBackgroundJobClientV2 _backgroundJobClientV2;
        private readonly IMemLedgerService _memLedgerService;
        private readonly IMediator _mediator;
        public RoSubAdvancedPaymentByMonthHandlerCommandHandler(IMemDbContext context, IPermissionHandler permissionHandler,
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
        public async Task<ListResult<MemberSubPaymentRes>> Handle(RoSubAdvancedPaymentByMonthCommand request, CancellationToken cancellation)
        {



            using (var transaction = await _context.BeginTransactionAsync(cancellation))
            {
                try
                {
                    var result = new ListResult<MemberSubPaymentRes>();
                    string subscriptionModes = "";
                    decimal totalPayment = 0;
                    decimal currentbalance = 0;
                    //var checkedList = request.Model.Model.Where(q => q.IsChecked).OrderBy(o => o.SubscriptionMonth).ToList();
                    //var uncheckedList = request.Model.Model.Where(q => !q.IsChecked).OrderBy(o => o.SubscriptionMonth).ToList();

                    //foreach (var item in checkedList)
                    //{
                    //    if (item.PaymentDate > uncheckedList.FirstOrDefault()?.PaymentDate)
                    //    {
                    //        result.HasError = true;
                    //        result.Messages?.Add("Please previous due payment first !");
                    //        return result;
                    //    }
                    //}
                    // need to fix



                    if (await _permissionHandler.IsTempMember())
                    {
                        result.HasError = true;
                        result.Messages?.Add("You have no permission to access, please contact with authority.");
                        return result;
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

                    if (member.PaidTill == null)
                    {
                        result.HasError = true;
                        result.Messages?.Add("Member subscription has not started yet!!");
                        return result;
                    }

                    var dueExist = _context.RoSubscriptionDueTemps
                        .AsNoTracking()
                        .Any(x => x.MemberId == request.MemberId && x.IsPaid == false);

                    if (dueExist)
                    {
                        result.HasError = true;
                        result.Messages?.Add("Please previous due payment first !");
                        return result;
                    }

                    // var lastPayemntNo = await _context.SubscriptionPaymentTemps.Select(s => s.PaymentNo).MaxAsync(cancellation);
                    var lastPayemntMonth = await _context.ROASubscriptionPaymentDetail.Where(s => s.MemberId == request.MemberId)
                        .OrderByDescending(s => s.SubscriptionMonth)
                        //   .Select(s => s.SubscriptionMonth)
                        .FirstOrDefaultAsync(cancellation);

                    if (lastPayemntMonth == null)
                    {
                        result.HasError = true;
                        result.Messages?.Add("Please contact to Administrator!!");
                        return result;
                    }

                    if (lastPayemntMonth!.SubscriptionMonth > request.Month)
                    {
                        result.HasError = true;
                        result.Messages?.Add("Please select the correct month!");
                        return result;
                    }

                    var monthList = GetMonthsBetweenDates(lastPayemntMonth!.SubscriptionMonth, request.Month);
                    var lastPayemntNo = await _context.ROASubscriptionPayments.Select(s => s.PaymentNo).MaxAsync(cancellation);
                    if (lastPayemntNo != "")
                    {
                        lastPayemntNo = (Convert.ToInt32(lastPayemntNo) + 1).ToString("00000000");
                    }
                    else
                    {
                        lastPayemntNo = "00000001";
                    }
                    //var member = await _context.RegisterMembers.SingleOrDefaultAsync(q => q.Id == request.Model.MemberId, cancellation);


                    // var subPays = new List<SubscriptionPaymentTemp>();
                    var subPays = new List<ROASubscriptionPaymentDetail>();
                    //  List<MemLedger> objList = new List<MemLedger>();
                    //  List<RoMemberLedger> objList = new List<RoMemberLedger>();
                    var payemnt = new ROASubscriptionPayment
                    {
                        MemberId = request.MemberId,
                        MemberShipNo = member?.MemberShipNo ?? "",
                        // MemberPayment = totalAmount,
                        PaymentNo = lastPayemntNo,
                        PaymentType = request.PaymentType,
                        //  LateAmount = s.LateAmount,
                        //  LateFeePer = s.LateFePer,
                        ActualPaymentDate = DateTime.UtcNow,
                        // PaymentDate = s.PaymentDate,
                        //   SubscriptionFeesId = s.Id,

                        IsPaid = true,
                        SubsStatus = "Paid",
                        //BankSlno = request.BankId,
                        //PaymentTypeID = request.PaymentTypeId,
                        //CardSlno = request.CardId,

                    };
                    _context.ROASubscriptionPayments.Add(payemnt);
                    foreach (var month in monthList)
                    {

                        var checkExist = _context.ROASubscriptionPaymentDetail
                        .AsNoTracking()
                        .Any(x => x.MemberId == request.MemberId && x.SubscriptionMonth == month && x.SubscriptionYear == month.Year);

                        decimal totalAmount = 0;

                        if (!checkExist)
                        {

                            var subDue = await _context.RoSubscriptionDueTemps
                                .AsNoTracking()
                                .AnyAsync(x => x.MemberId == request.MemberId
                                                    && x.SubscriptionMonth == month
                                                    && x.SubscriptionYear == month.Year
                            //  && x.IsPaid == false
                            );
                            if (!subDue)
                            {
                                totalAmount += member.SubscriptionFee.GetValueOrDefault();

                                var paymentDetail = new ROASubscriptionPaymentDetail()
                                {
                                    MemberId = request.MemberId,
                                    MemberShipNo = member?.MemberShipNo ?? "",
                                    PaymentFees = member!.SubscriptionFee.GetValueOrDefault(),
                                    PaymentNo = lastPayemntNo,
                                    LateFees = 0,// s.LateAmount,
                                    //  LateFeePer = s.LateFePer,
                                    ActualPaymentDate = DateTime.Now,
                                    PaymentFeesDate = month,
                                    SubscriptionFeesId = 1, // s.Id,
                                    SubscriptionName = "Montly", // s.SubscriptionName ?? "",
                                    SubscriptionYear = month.Year, // s.PaymentYear,
                                    IsPaid = true,
                                    //SubsStatus = "Paid",
                                    SubscriptionMonth = month,
                                    SubscriptionPaymentId = payemnt.Id
                                };
                                subPays.Add(paymentDetail);



                                // subDue.IsPaid = true;
                                //  _context.RoSubscriptionDueTemps.Update(subDue);
                            }


                        }
                    }

                    _context.ROASubscriptionPaymentDetail.AddRange(subPays);
                    if (await _context.SaveChangesAsync(cancellation) > 0)
                    {
                        if (member != null)
                        {

                            string message = "";
                            string subject = "";

                            var lasMonth = monthList.OrderByDescending(o => o).First();
                            int lastDay = DateTime.DaysInMonth(lasMonth.Year, lasMonth.Month);

                            member.PaidTill = new DateTime(lasMonth.Year, lasMonth.Month, lastDay); ;
                            //  _context.RoMemberLedgers.AddRange(objList);
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

            static List<DateTime> GetMonthsBetweenDates(DateTime start, DateTime end)
            {
                List<DateTime> months = new List<DateTime>();

                DateTime current = new DateTime(start.Year, start.Month, 1).AddMonths(1);

                while (current <= end)
                {
                    months.Add(current);
                    current = current.AddMonths(1);
                }

                return months;
            }

        }

    }
}
