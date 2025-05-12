using Hangfire;
using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Interfaces;
using MemApp.Application.Mem.TopUps.Models;
using MemApp.Application.MessageInboxs.Commands.MessageGenerateCommand;
using MemApp.Application.MessageInboxs.Models;
using MemApp.Domain.Entities.com;
using MemApp.Domain.Entities.mem;
using MemApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MemApp.Application.Models.DTOs;
using Res.Domain.Entities;

namespace ResApp.Application.Com.Commands.RoTopUp
{
    
    public class CreateRoTopUpCommand : IRequest<TopUpVm>
    {
        public TopUpReq Model { get; set; } = new TopUpReq();

        public DateTime StartingMonth { get; set; }
        public DateTime EndingMonth { get; set; }

        public int MonthCount {  get; set; }
    }

    public class CreateRoTopUpCommandHandler : IRequestHandler<CreateRoTopUpCommand, TopUpVm>
    {
        private readonly IMemDbContext _context;
        private readonly IMemLedgerService _memLedgerService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IMediator _mediator;
        private readonly IBackgroundJobClientV2 _backgroundJobClientV2;
        public CreateRoTopUpCommandHandler(
            IMemDbContext context,
            IMemLedgerService memLedgerService,
            ICurrentUserService currentUserService,
            IBackgroundJobClient backgroundJobClient,
            IMediator mediator,
            IBackgroundJobClientV2 backgroundJobClientV2
            )
        {
            _context = context;
            _memLedgerService = memLedgerService;
            _currentUserService = currentUserService;
            _backgroundJobClient = backgroundJobClient;
            _mediator = mediator;
            _backgroundJobClientV2 = backgroundJobClientV2;
        }
        public async Task<TopUpVm> Handle(CreateRoTopUpCommand request, CancellationToken cancellation)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            var result = new TopUpVm();
          //  decimal curentBalance = 0;
            try
            {
                if (request.Model.TopUpDetails.Count > 1)
                {
                    result.HasError = true;
                    result.Succeeded = false;
                    result.Messages?.Add("somrthing went wrong please Reload or clean cache");
                    return result;
                }
                if (request.Model.TopUpDetails.Count == 0)
                {
                    result.HasError = true;
                    result.Succeeded = false;
                    result.Messages?.Add("Please select payment method ");
                    return result;
                }

                if (request.Model.TopUpDetails.Where(q => q.PaymentMethodId == 0).Count() > 0)
                {
                    result.HasError = true;
                    result.Succeeded = false;
                    result.Messages?.Add("Please select payment method ");
                    return result;
                }
                string cardNo = "";

                //var memberObj = await _context.RegisterMembers
                //    .Select(s => new { s.Id, s.Phone, s.Email, s.MembershipNo, s.IsMasterMember, s.CardNo })
                //   .FirstOrDefaultAsync(q => q.Id == request.Model.RegisterMemberId, cancellation);

                var memberObj = await _context.MemberRegistrationInfos
                   .Select(s => new { s.Id, s.PhoneNo, s.Email, s.MemberShipNo /*, s.IsMasterMember, s.CardNo*/ })
                  .FirstOrDefaultAsync(q => q.Id == request.Model.MemberId, cancellation);

                //if (memberObj != null ? memberObj.IsMasterMember == true : false)
                //{
                //    request.Model.RegisterMemberId = memberObj.Id;
                //    cardNo = memberObj.CardNo ?? "";
                //}
                //else
                //{
                //    var smemberObj = await _context.RegisterMembers
                //          .Select(s => new { s.Id, s.Phone, s.Email, s.MembershipNo, s.IsMasterMember, s.MemberId, s.CardNo })
                //     .FirstOrDefaultAsync(q => q.Id == request.Model.RegisterMemberId, cancellation);

                //    if (smemberObj != null)
                //    {
                //        request.Model.RegisterMemberId = smemberObj.MemberId ?? 0;
                //        cardNo = smemberObj.CardNo ?? "";
                //    }
                //}

                var obj = await _context.TopUps
                        .Include(i => i.TopUpDetails)
                        .SingleOrDefaultAsync(q => q.Id == request.Model.Id, cancellation);


                if (obj == null)
                {
                    obj = new TopUp();
                    if (request.Model.OfflineTopUp)
                    {
                        obj.OfflineTopUp = true;
                    }
                    else if (request.Model.OnlineTopUp)
                    {
                        obj.OnlineTopUp = true;
                    }
                    obj.Status = request.Model.Status;
                    obj.IsActive = true;
                    obj.MemberShipNo = memberObj!.MemberShipNo!; // request.Model.MemberShipNo ?? "";
                    obj.CardNo = request.Model.CardNo;
                  //  obj.RegisterMemberId = request.Model.RegisterMemberId;
                    obj.MemberId = request.Model.MemberId;
                    obj.TopUpDate = DateTime.Now;
                    _context.TopUps.Add(obj);
                }

                obj.TotalAmount = request.Model.TopUpDetails.Sum(s => s.Amount);
                obj.Note = request.Model.Note;
                obj.PaymentMode = request.Model.PaymentMode;


                if (await _context.SaveChangesAsync(cancellation) > 0)
                {
                    List<TopUpDetail> details = new List<TopUpDetail>();


                    foreach (var d in request.Model.TopUpDetails)
                    {
                        var td = new TopUpDetail()
                        {
                            PaymentMethodId = d.PaymentMethodId,
                            PaymentMethodText = d.PaymentMethodText ?? "",
                            Amount = d.Amount,
                            TrxCardNo = d.TrxCardNo,
                            MachineNo = d.MachineNo,
                            TrxNo = d.TrxNo,
                            IsActive = true,
                            TopUpId = obj.Id,
                            BankId = d.BankId,
                            BankText = d.BankText,
                            CreditCardId = d.CreditCardId,
                            CreditCardText = d.CreditCardText
                        };
                        details.Add(td);
                    }
                    _context.TopUpDetails.AddRange(details);
                    if (await _context.SaveChangesAsync(cancellation) > 0)
                    {
                        List<RoMemberLedger> ledgers = new List<RoMemberLedger>();
                        foreach (var d in obj.TopUpDetails)
                        {
                            string preFix = "T";
                            var topUpNo = "";
                            var max = _context.MemLedgers.Where(q => q.TOPUPID.StartsWith(preFix))
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
                              //  PrvCusID = obj.RegisterMemberId.ToString(),
                                MemberId = obj.MemberId.ToString(),
                                Amount = d.Amount,
                               // Description = "TOP UP By : Member Ship No :" + request.Model.MemberShipNo + ", Card No : " + cardNo,
                                Description = "Subscription fee Details : " + request.MonthCount + " Months, " + request.StartingMonth.ToString("MMMM, yyyyy") + " To  " +
                                                           request.EndingMonth.ToString("MMMM, yyyy"),
                                Dates = DateTime.Now,
                                TOPUPID = topUpNo,
                                DatesTime = DateTime.Now,
                                PayType = d.PaymentMethodText,
                                BankCreditCardName = d.BankText,
                                ChequeCardNo = d.TrxCardNo,
                                // Notes = "TOPUP : TopUpId :" + d.Id + " Note : " + obj.Note + " " + d.BankText + "" + d.TrxNo,
                                Notes = "Subscription fee Details : " + request.MonthCount + " Months, " + request.StartingMonth.ToString("MMMM, yyyyy") + " To  " +
                                                           request.EndingMonth.ToString("MMMM, yyyy"),
                                ServiceChargeAmount = 0,
                                RefundId = "",
                                TransactionFrom = _currentUserService.AppId,
                                TransactionType = "TOPUP",
                                UpdateBy = _currentUserService.Username,
                                UpdateDate = DateTime.Now,
                                TopUpDetailId = d.Id
                            };
                            ledgers.Add(lObj);
                        }
                        _context.RoMemberLedgers.AddRange(ledgers);
                        await _context.SaveChangesAsync(cancellation);
                        if (memberObj != null)
                        {
                          //  curentBalance = await _memLedgerService.GetCurrentBalance(request.Model.RegisterMemberId.ToString());
                            string message = "";
                            string subject = "";
                            //if (memberObj.Phone != null)
                            //{
                            //    message = "Dear CCCL Member, Tk. " + Math.Round(obj.TotalAmount, 2) + " has been added to your wallet, current balance is Tk. " + Math.Round(curentBalance, 2) + ", Thanks.";
                            //    var SMS = new SMSInfo
                            //    {
                            //        Message = message,
                            //        Phone = memberObj.Phone
                            //    };
                            //    _backgroundJobClient.Enqueue<IBroadcastHandler>(e => e.SendSms(memberObj.Phone, message, "English", null, null));

                            //    message = "";
                            //}

                            //if (memberObj.Email != null)
                            //{
                            //    message = "Dear CCCL Member, Tk. " + Math.Round(obj.TotalAmount, 2) + " has been added to your wallet, current balance is Tk. " + Math.Round(curentBalance, 2) + ", Thanks.";
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

                            //var messageObj = new MessageInboxCreateDto
                            //{
                            //    MemberId = obj.RegisterMemberId,
                            //    Message = "Dear CCCL Member, Tk. " + Math.Round(obj.TotalAmount, 2) + " has been added to your wallet, current balance is Tk. " + Math.Round(curentBalance, 2) + ", Thanks.",
                            //    TypeId = MessageInboxTypeEnum.Topup,
                            //    IsRead = false,
                            //    IsAllMember = false,

                            //};
                            //_backgroundJobClientV2.Enqueue(() => ProcessMessage(new MessageGenerateCommand() { Model = messageObj }));
                        }
                    }




                    result.HasError = false;
                    result.Succeeded = true;
                    result.Data.Id = obj.Id;
                    result.Messages?.Add("Top Up created.");
                }
                else
                {
                    result.HasError = true;
                    result.Succeeded = false;
                    result.Messages?.Add("Top Up Failed.");
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Succeeded = false;
                result.Messages?.Add("Exception" + ex.Message);
            }
            return result;
        }

        public async Task ProcessMessage(MessageGenerateCommand command)
        {
            await _mediator.Send(command);
        }


    }
}
