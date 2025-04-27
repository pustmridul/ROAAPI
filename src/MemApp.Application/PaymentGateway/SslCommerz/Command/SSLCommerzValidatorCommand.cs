using Hangfire;
using MediatR;
using MemApp.Application.Exceptions;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.PaymentGateway.SslCommerz.Model;
using MemApp.Domain.Entities.com;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Res.Domain.Entities;
using ResApp.Application.Com.Commands.ROAPayment;
using ResApp.Application.Com.Commands.ROAPayment.Models;
using ResApp.Application.ROA.MembershipFee.Command;
using ResApp.Application.ROA.MembershipFee.Models;

namespace MemApp.Application.PaymentGateway.SslCommerz.Command
{
    public class SSLCommerzValidatorCommand : IRequest<Result>
    {
        public SSLCommerzValidatorResponseReq Model { get; set; } = new SSLCommerzValidatorResponseReq();
    }
    public class SSLCommerzValidatorCommandHandler : IRequestHandler<SSLCommerzValidatorCommand, Result>
    {
        private readonly IMemDbContext _context;
        private readonly IMemLedgerService _memLedgerService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IBroadcastHandler _broadcastHandler;
        private readonly IBackgroundJobClient _backgroundJobClient; 
        private readonly IMediator _mediator;
        public SSLCommerzValidatorCommandHandler(IBroadcastHandler broadcastHandler, IMemDbContext context,
            IMemLedgerService memLedgerService, ICurrentUserService currentUserService, IBackgroundJobClient backgroundJobClient,   IMediator mediator)
        {
            _context = context;
            _memLedgerService = memLedgerService;
            _currentUserService = currentUserService;
            _backgroundJobClient = backgroundJobClient;
            _broadcastHandler = broadcastHandler;
            _mediator = mediator;
        }

        public async Task<Result> Handle(SSLCommerzValidatorCommand request, CancellationToken cancellationToken)
        {
            try
            {
                string paymentNo=string.Empty;
                if (request.Model.tran_id != null)
                {
                    var result = new Result();
                    var tobj = await _context.TopUps
                        .Include(i => i.TopUpDetails)
                        .SingleOrDefaultAsync(q => q.Note!.Trim() == request.Model.tran_id.Trim(), cancellationToken);

                    if (tobj == null)
                    {
                        throw new NotFoundException();
                    }
                    else
                    {
                        if (tobj.Status == "Confirm" && tobj.IsActive)
                        {
                            result.HasError = false;
                            result.Messages?.Add("Transaction Already Completed");
                            return result;
                        }
                        List<RoMemberLedger> ledgers = new List<RoMemberLedger>();
                        tobj.IsActive = true;
                        tobj.Status = "Confirm";
                        tobj.PaymentMode = request.Model.card_brand;
                        if (tobj.PaymentFor == "Membership Fee")
                        {
                            var paymentMembershipFee = await _mediator.Send(new RoMembershipFeePaymentCommand()
                            {
                                Model = new MembershipFeePayReq
                                {
                                    Amount=tobj.TotalAmount,
                                    MemberId = tobj.MemberId,
                                },
                                IsOnline=true
                                
                            });
                            paymentNo = paymentMembershipFee.Data.PaymentNo;
                        }
                        var paidMonths = new List<PaymentTracking>();
                        if (tobj.PaymentFor == "Subscription Fee")
                        {
                             paidMonths = JsonConvert.DeserializeObject<List<PaymentTracking>>(tobj.MonthDetails!)?
                                                        .OrderBy(x => x.SubscriptionMonth)
                                                        .ToList() ?? new List<PaymentTracking>();

                            var payment = await _mediator.Send(new ROPaymentOnlineCommand()
                            {
                                Model = paidMonths!,
                                MemberId = tobj.MemberId
                            });
                            paymentNo = payment.Data.PaymentNo;
                        }
                            
                       
                        foreach (var d in tobj.TopUpDetails)
                        {
                            d.IsActive = true;
                            d.TrxNo = request.Model.bank_tran_id;
                            d.TrxCardNo = request.Model.bank_tran_id;
                            d.PaymentMethodText = request.Model.card_issuer ?? "";

                            string preFix = "T";
                            var topUpNo = "";
                            var max = _context.RoMemberLedgers.Where(q => q.TOPUPID.StartsWith(preFix))
                                .Select(s => s.TOPUPID.Replace(preFix, "")).DefaultIfEmpty().Max();

                            if (string.IsNullOrEmpty(max))
                            {
                                topUpNo = preFix + "0000001";
                            }
                            else
                            {
                                topUpNo = preFix + (Convert.ToInt32(max) + 1).ToString("0000000");
                            }
                            var lObj = new RoMemberLedger()
                            {
                                ReferenceId = topUpNo,
                                MemberId = tobj.MemberId.ToString(),
                                Amount = Math.Round(d.Amount),
                                //  Description = "TOP UP By : Member Ship No :" + tobj.MemberShipNo + ", Card No : " + tobj.CardNo,
                                // Description = "Subscription Bill By : Member Ship No :" + tobj.MemberShipNo ,
                                //Description = "Subscription fee Details : " + paidMonths.Count + " Months, " + paidMonths.FirstOrDefault()!.SubscriptionMonth.ToString("MMMM, yyyy") + " To  " +
                                //                           paidMonths.LastOrDefault()!.SubscriptionMonth.ToString("MMMM, yyyy"),
                                Dates = DateTime.Now,
                                TOPUPID = topUpNo,
                                UpdateBy = tobj.CreatedByName,
                                UpdateDate = DateTime.Now,
                                DatesTime = DateTime.Now,
                                PayType = "SSLCOMMERZ",
                                BankCreditCardName = d.BankText,
                                ChequeCardNo = d.PaymentMethodText,
                               // Notes = "TOPUP : TopUpId :" + d.Id + " Transaction Id : " + tobj.Note,
                                //Notes= "Subscription fee Details : " + paidMonths.Count + " Months, " + paidMonths.FirstOrDefault()!.SubscriptionMonth.ToString("MMMM, yyyy") + " To  " +
                                //                           paidMonths.LastOrDefault()!.SubscriptionMonth.ToString("MMMM, yyyy"),
                                ServiceChargeAmount = 0,
                                RefundId = "",
                                TransactionType = "TOPUP",
                                TransactionFrom = "MOBILEAPP",
                                TopUpDetailId = d.Id,
                            };

                            if (tobj.PaymentFor == "Membership Fee")
                            {
                                lObj.Description = "Membership Fee";
                                lObj.Notes = "Membership Fee";
                            }

                            if (tobj.PaymentFor == "Subscription Fee")
                            {
                                lObj.Description = "Subscription fee Details : " + paidMonths.Count + " Months, " + paidMonths.FirstOrDefault()!.SubscriptionMonth.ToString("MMMM, yyyy") + " To  " +
                                                           paidMonths.LastOrDefault()!.SubscriptionMonth.ToString("MMMM, yyyy");
                                lObj.Notes = "Subscription fee Details : " + paidMonths.Count + " Months, " + paidMonths.FirstOrDefault()!.SubscriptionMonth.ToString("MMMM, yyyy") + " To  " +
                                                           paidMonths.LastOrDefault()!.SubscriptionMonth.ToString("MMMM, yyyy");
                            }

                            ledgers.Add(lObj);
                            _context.RoMemberLedgers.AddRange(ledgers);
                        }
                        await _context.SaveChangesAsync(cancellationToken);

                        //var memberObj = await _context.RegisterMembers
                        //            .Select(s => new { s.Phone, s.Email, s.Id, s.MembershipNo, s.PrvCusID })
                        //            .SingleOrDefaultAsync(q => q.Id == tobj.MemberId, cancellationToken);

                        var memberObj = await _context.MemberRegistrationInfos
                                  .Select(s => new { s.PhoneNo, s.Email, s.Id, s.MemberShipNo })
                                  .SingleOrDefaultAsync(q => q.Id == tobj.MemberId, cancellationToken);

                        if (memberObj != null)
                        {
                          //  var curentBalance = await _memLedgerService.GetCurrentBalance(memberObj.Id.ToString());
                            string message = "";
                            string subject = "";

                            //if (memberObj.Phone != null)
                            //{
                            //    message = "Dear CCCL Member, Tk. " + Math.Round(tobj.TotalAmount, 2) + " has been added to your wallet, current balance is Tk. " + Math.Round(curentBalance, 2) + ", Thanks.";
                            //    var SMS = new SMSInfo
                            //    {
                            //        Message = message,
                            //        Phone = memberObj.Phone
                            //    };
                            //    _backgroundJobClient.Enqueue<IBroadcastHandler>(e => e.SendSms(memberObj.Phone, message, "English", null, null));

                            //}

                            //if (memberObj.Email != null)
                            //{
                            //    message = "Dear CCCL Member, Tk. " + Math.Round(tobj.TotalAmount, 2) + " has been added to your wallet, current balance is Tk. " + Math.Round(curentBalance, 2) + ", Thanks.";
                            //    subject = "Wallet amount added (Cadet College Club Ltd) ";

                            //    var EmailInfo = new EmailInfo
                            //    {
                            //        MailBody = message,
                            //        MailSubject = subject,
                            //        MailId = memberObj.Email
                            //    };
                            //    _backgroundJobClient.Enqueue<IBroadcastHandler>(e => e.SendEmail(memberObj.Email, subject, message, null, null));
                            //    message = "";
                            //    subject = "";
                            //}
                        }

                        var obj = _context.SSLCommerzValidators
                          .SingleOrDefault(q => q.tran_id == request.Model.tran_id);

                        if (obj == null)
                        {
                            obj = new SSLCommerzValidator()
                            {
                                APIConnect = request.Model.APIConnect,
                                status = request.Model.status,
                                sessionkey = request.Model.sessionkey,
                                tran_date = request.Model.tran_date,
                                tran_id = request.Model.tran_id,
                                val_id = request.Model.val_id,
                                amount = request.Model.amount,
                                store_amount = request.Model.store_amount,
                                card_type = request.Model.card_type,
                                card_no = request.Model.card_no,
                                currency = request.Model.currency,
                                bank_tran_id = request.Model.bank_tran_id,
                                card_issuer = request.Model.card_issuer,
                                card_brand = request.Model.card_brand,
                                card_issuer_country_code = request.Model.card_issuer_country_code,
                                card_issuer_country = request.Model.card_issuer_country,
                                currency_type = request.Model.currency,
                                currency_amount = request.Model.currency_amount,
                                emi_amount = request.Model.emi_amount,
                                emi_instalment = request.Model.emi_instalment,
                                discount_percentage = request.Model.discount_percentage,
                                discount_remarks = request.Model.discount_remarks,
                                value_a = request.Model.value_a,
                                value_b = request.Model.value_b,
                                value_c = request.Model.value_c,
                                value_d = request.Model.value_d,
                                risk_level = request.Model.risk_level,
                                risk_title = request.Model.risk_title
                            };

                            _context.SSLCommerzValidators.Add(obj);
                            _context.Save();
                        }
                    }

                    //var redirectUrl = $"http://36.255.71.72:177/member_profile" +
                    //      $"?paymentNo={paymentNo}&msg=Payment successful!";

                    //return Redirect(redirectUrl);
                    result.HasError = false;
                    result.Messages?.Add("Save Success");
                    result.Messages?.Add(paymentNo);
                    return result;
                }
                else
                {
                    throw new Exception("tran_id is null");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}
