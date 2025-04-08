using Hangfire;
using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Interfaces;
using Res.Domain.Entities.ROAPayment;
using ResApp.Application.Com.Commands.ROAPayment.Models;
using ResApp.Application.Com.Commands.RoTopUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResApp.Application.Com.Commands.RoaMembershipFee.Models;
using Microsoft.EntityFrameworkCore;

namespace ResApp.Application.Com.Commands.RoaMembershipFee
{
    
    public class RoMembershipFeePaymentCommand : IRequest<Result<MembershipFeePymentRes>>
    {
        //  public SubscriptionFeeModel Model { get; set; } = new SubscriptionFeeModel();
        public MembershipFeePayReq Model { get; set; } = new MembershipFeePayReq();

    }

    public class RoMembershipFeePaymentCommandHandler : IRequestHandler<RoMembershipFeePaymentCommand, Result<MembershipFeePymentRes>>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        private readonly ICurrentUserService _currentUserService;
        private readonly IBroadcastHandler _broadcastHandler;
        private readonly IBackgroundJobClientV2 _backgroundJobClientV2;
        private readonly IMemLedgerService _memLedgerService;
        private readonly IMediator _mediator;
        public RoMembershipFeePaymentCommandHandler(IMemDbContext context, IPermissionHandler permissionHandler,
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
        public async Task<Result<MembershipFeePymentRes>> Handle(RoMembershipFeePaymentCommand request, CancellationToken cancellation)
        {
            var result = new Result<MembershipFeePymentRes>
            {
                Data= new MembershipFeePymentRes()
            };

            var exist=_context.ROAMembershipFeePayments.Any(x=>x.MemberId==request.Model.MemberId);
            if (exist)
            {
                result.HasError = true;
                result.Messages?.Add("Member Fee has been paid already!!");
                return result;
            }

            using (var transaction = await _context.BeginTransactionAsync(cancellation))
            {
                try
                {
                    // var result = new MemberSubFeeListVm();
                 
                   
                   
                    
                   
                    var lastPayemntNo = await _context.ROAMembershipFeePayments.Select(s => s.PaymentNo).MaxAsync(cancellation);
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

                   

                    var payemnt = new RoaMembershipFeePayment
                    {
                        MemberId = request.Model.MemberId,
                       // MemberShipNo = member?.MemberShipNo ?? "",
                        Amount=request.Model.Amount,
                        PaymentNo = lastPayemntNo,
                      
                        PaymentDate = DateTime.Now,
                     

                       

                    };
                   
                    _context.ROAMembershipFeePayments.Add(payemnt);

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

                   

                 //   if (subPays.Count > 0)
                //    {
                       

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


                       
                        result.Data.PaymentNo = payemnt.PaymentNo;

                        result.HasError = false;
                        result.Messages?.Add("Payment has been done successfully");
                        return result;
                //    }

                  

                }
                catch (Exception ex)
                {
                    result.HasError = true;
                    result.Messages?.Add("Something went wrong!!!");
                    return result;
                    throw new Exception(ex.Message);
                }
            }

        }

    }
}
