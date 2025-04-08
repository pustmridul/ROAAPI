using FirebaseAdmin.Messaging;
using Hangfire;
using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.ServiceSales.Models;
using MemApp.Application.Mem.TopUps.Command;
using MemApp.Application.MessageInboxs.Commands.MessageGenerateCommand;
using MemApp.Application.MessageInboxs.Models;
using MemApp.Application.Models;
using MemApp.Domain.Entities.Communication;
using MemApp.Domain.Entities.mem;
using MemApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace MemApp.Application.Mem.ServiceSales.Command
{
    public class CreateServiceSaleCommand : IRequest<ServiceSaleVm>
    {
        public ServiceSaleReq Model { get; set; } = new ServiceSaleReq();
    }

    public class CreateServiceSaleCommandHandler : IRequestHandler<CreateServiceSaleCommand, ServiceSaleVm>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        private readonly IUserLogService _userLogService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionHandler _permissionHandler;
        private readonly IMemLedgerService _memLedgerService;
        private readonly IBroadcastHandler _broadcastHandler;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IBackgroundJobClientV2 _backgroundJobClientV2;
        public CreateServiceSaleCommandHandler(
            IMemDbContext context,
            IMediator mediator,
            IUserLogService userLogService,
            ICurrentUserService currentUserService,
            IPermissionHandler permissionHandler,
            IMemLedgerService memLedgerService,
            IBroadcastHandler broadcastHandler,
            IBackgroundJobClient backgroundJobClient,
            IBackgroundJobClientV2 backgroundJobClientV2
            )
        {
            _context = context;
            _mediator = mediator;
            _userLogService = userLogService;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
            _memLedgerService = memLedgerService;
            _broadcastHandler = broadcastHandler;
            _backgroundJobClient = backgroundJobClient;
            _backgroundJobClientV2 = backgroundJobClientV2;
        }
        public async Task<ServiceSaleVm> Handle(CreateServiceSaleCommand request, CancellationToken cancellation)
        {



            var result = new ServiceSaleVm();
            var ticketList = new List<string>();

            if (await _permissionHandler.IsTempMember())
            {
                result.HasError = true;
                result.Messages?.Add("You have no permission to access, please contact with authority.");
                return result;
            }
            try
            {
                //if (DateTime.Parse(request.Model.InvoiceDate).Date < DateTime.Now.Date)
                //{
                //    result.HasError = true;
                //    result.Messages?.Add("Invoice Date is not valid");
                //    return result;
                //}
                var memberObj = await _context.RegisterMembers.Select(s => new { s.Id, s.Email, s.Phone, s.MembershipNo, s.CardNo,s.FullName })
                                      .FirstOrDefaultAsync(q => q.Id == request.Model.MemberId, cancellation);
                if (memberObj == null)
                {
                    result.HasError = true;
                    result.Messages?.Add("Member not found");
                    return result;
                }

                var curentBalance = await _memLedgerService.GetCurrentBalance(memberObj.Id.ToString());
                if (curentBalance <= 0)
                {
                    if (request.Model.topup?.TotalAmount < request.Model.TotalAmount)
                    {
                        result.HasError = true;
                        result.Messages?.Add("Top up amount is not enough");
                        return result;
                    }
                }


                //Topup
                if (request.Model.topup is not null)
                {
                    request.Model.topup.RegisterMemberId = memberObj.Id;
                    var topUpResult = await _mediator.Send(new CreateTopUpCommand() { Model = request.Model.topup });

                    //if top is no succceeded then it will not proceed to execute the rest of the code
                    if (topUpResult.Succeeded == false || topUpResult.HasError == true)
                    {
                        result.HasError = true;
                        result.Succeeded = false;
                        result.Messages = topUpResult.Messages;
                        return result;
                    }
                }

                curentBalance = await _memLedgerService.GetCurrentBalance(memberObj.Id.ToString());


                foreach (var d in request.Model.ServiceSaleDetailReqs.Where(q => q.SeviceTicketAvailablityId != 0))
                {
                    var soldqty = await _context.ServiceSalesDetails
                                .Where(q => q.SeviceTicketAvailablityId == d.SeviceTicketAvailablityId
                                && q.RevDate.Date == new DateTime(d.SaleYear, d.SaleMonth, d.SaleDay))
                                .SumAsync(s => s.Quantity, cancellation);
                    var av = d.SlotQty - (d.Quantity + soldqty);
                    if (av < 0)
                    {
                        result.HasError = true;
                        result.Messages?.Add(d.TicketText + " is not available");

                    }

                }
                if (result.HasError)
                {
                    return result;
                }

                //Service sale create
                var obj = await _context.ServiceSales
               .Include(i => i.ServiceSaleDetails)
               .SingleOrDefaultAsync(q => q.Id == request.Model.Id, cancellation);


                if (obj == null)
                {
                    DateTime invoiceDateTime = DateTime.Now;
                    string preFix = "S" + invoiceDateTime.ToString("yyMMdd");

                    var max = _context.ServiceSales.Where(q => q.InvoiceNo.StartsWith(preFix))
                        .Select(s => s.InvoiceNo.Replace(preFix, "")).DefaultIfEmpty().Max();

                    obj = new ServiceSale();
                    if (string.IsNullOrEmpty(max))
                    {
                        obj.InvoiceNo = preFix + "0001";
                    }
                    else
                    {
                        obj.InvoiceNo = preFix + (Convert.ToInt32(max) + 1).ToString("0000");
                    }

                    obj.MemberId = request.Model.MemberId;
                    obj.InvoiceDate = DateTime.Now;
                    obj.MembershipNo = request.Model.MembershipNo;
                    obj.OrderFrom = request.Model.OrderFrom;
                    obj.IsActive = true;
                    _context.ServiceSales.Add(obj);
                    result.HasError = false;
                    result.Messages?.Add("New ServiceSale Created");

                }


                obj.Note = request.Model.Note;


                //Service sale detail create
                if (await _context.SaveChangesAsync(cancellation) > 0)
                {
                    decimal lineTotal = 0;
                    decimal vatTotal = 0;
                    decimal serviceChargeTotal = 0;
                    int i = 1;
                    foreach (var d in request.Model.ServiceSaleDetailReqs)
                    {
                        string ticketCode = "";
                        var objD = new ServiceSaleDetail();
                        objD.RevYear = d.SaleYear;
                        objD.RevMonth = d.SaleMonth;
                        objD.RevDay = d.SaleDay;
                        objD.RevDate = new DateTime(d.SaleYear, d.SaleMonth, d.SaleDay);
                        objD.ServiceSaleId = obj.Id;
                        objD.ServiceTicketId = d.ServiceTicketId;
                        objD.MemServiceId = d.MemServiceId;
                        objD.IsActive = true;
                        objD.UnitName = d.UnitName;
                        objD.UnitPrice = d.UnitPrice;
                        objD.Quantity = d.Quantity;
                        if (d.Quantity > 1)
                        {

                            var qty = d.Quantity;
                            while (qty > 0)
                            {
                                ticketCode += obj.Id.ToString("0000") + "-" + i.ToString("00") + ",";
                                qty--;
                                i++;
                            }
                        }
                        else
                        {
                            ticketCode = obj.Id.ToString("0000") + "-" + i.ToString("00");
                            i++;
                        }

                        objD.TicketCodeNo = ticketCode;// GenerateRandomAlphanumericString(new Random());
                        objD.VatChargePercent = d.VatChargePercent;
                        objD.VatChargeAmount = (d.UnitPrice * (d.VatChargePercent / 100)) * d.Quantity ?? 0;
                        objD.ServiceChargeAmount = (d.UnitPrice * (d.ServiceChargePercent / 100)) * d.Quantity ?? 0;
                        objD.ServiceCriteriaId = d.ServiceCriteriaId;
                        objD.ServiceCriteriaText = d.ServiceCriteriaText;
                        objD.UnitPrice = d.UnitPrice;
                        objD.TicketText = d.TicketText;
                        objD.DayText = d.DayText;
                        objD.StartTime = d.StartTime;
                        objD.EndTime = d.EndTime;
                        objD.IsWholeDay = d.IsWholeDay;
                        objD.SeviceTicketAvailablityId = d.SeviceTicketAvailablityId;
                        ticketList.Add(d.TicketText ?? "");
                        _context.ServiceSalesDetails.Add(objD);
                        lineTotal += (decimal)(objD.Quantity * objD.UnitPrice);
                        vatTotal += objD.VatChargeAmount;
                        serviceChargeTotal += objD.ServiceChargeAmount;


                    }
                    obj.Amount = lineTotal;
                    obj.VatAmount = vatTotal;
                    obj.ServiceChargeAmount = serviceChargeTotal;
                    obj.TotalAmount = lineTotal + vatTotal + serviceChargeTotal;

                    await _context.SaveChangesAsync(cancellation);


                    // Ledger transaction
                    if (obj.TotalAmount > 0)
                    {
                        var memLedger = new MemLedgerVm()
                        {
                            ReferenceId = obj.InvoiceNo,
                            Amount = (-1) * obj.TotalAmount,
                            Dates = obj.InvoiceDate,
                            PrvCusID = obj.MemberId.ToString() ?? "",
                            TOPUPID = "",
                            UpdateBy = _currentUserService.Username,
                            UpdateDate = DateTime.Now,
                            DatesTime = DateTime.Now,
                            Notes = "ServiceSale : SaleId : " + obj.Id,
                            PayType = "",
                            Description = "Member # " + memberObj?.MembershipNo + ", Card No : " + memberObj?.CardNo + ", Service " + string.Join(", ", ticketList) + ",  Invoice : " + obj.InvoiceNo + ",Amount : " + obj.Amount + ", VAT : " + obj.VatAmount + ", Service Charge : " + obj.ServiceChargeAmount,
                            TransactionFrom = _currentUserService.AppId,
                            TransactionType = "SERVICE"
                        };

                        await _memLedgerService.CreateMemLedger(memLedger);

                        if (memberObj != null)
                        {
                            //Admin Notification saved
                            var adminNotification = new AdminNotification
                            {
                                Message = $"A service has been sold! Member: {memberObj.FullName}, Service: {obj.ServiceSaleDetails.First().TicketText}",
                                TypeId = MessageInboxTypeEnum.ServiceTicketSale,
                                IsRead = false,
                            };
                            _context.AdminNotifications.Add(adminNotification);
                            await _context.SaveChangesAsync(cancellation);

                            //Admin mail send
                            var notificationEmail = await _context.NotificationEmails.FirstOrDefaultAsync();
                            _backgroundJobClientV2.Enqueue<IBroadcastHandler>(e => e.SendEmail(notificationEmail.ServiceSaleNotificationEmail, "Service Sale", adminNotification.Message, null, null));


                            string message = "";
                            string subject = "";

                            if (memberObj.Phone != null)
                            {

                                message = "Dear CCCL Member, Tk. " + Math.Round(obj.TotalAmount, 2) + " has been deducted from your wallet, current balance is Tk. " + Math.Round(curentBalance, 2) + ", Thanks.";
                               
                                result.Data.SmsText = message;
                              //  _backgroundJobClient.Enqueue<IBroadcastHandler>(e => e.SendSms(memberObj.Phone, message, "English"));
                               // message = "";
                            }
                            if (memberObj.Email != null)
                            {
                                message = "Dear CCCL Member, Tk. " + Math.Round(obj.TotalAmount, 2) + " has been deducted from your wallet, current balance is Tk. " + Math.Round(curentBalance, 2) + ", Thanks.";

                                // _backgroundJobClient.Enqueue<IBroadcastHandler>(e => e.SendEmail(memberObj.Email, subject, message));
                                // message = "";
                                result.Data.SmsText = message;
                            }
                            if (request.Model.Id == null || request.Model.Id == 0)
                            {
                                var messageObj = new MessageInboxCreateDto
                                {
                                    MemberId = Convert.ToInt32(obj.MemberId),
                                    Message = message,
                                    TypeId = MessageInboxTypeEnum.ServiceTicketSale,
                                    IsRead = false,
                                    IsAllMember = false,

                                };
                                _backgroundJobClientV2.Enqueue(() => ProcessMessage(new MessageGenerateCommand() { Model = messageObj }));
                            }

                        }
                    }

                   

                    result.Data.PhoneNo = memberObj?.Phone;
                    result.Data.Id = obj.Id;
                    result.Data.MemberEmail = memberObj?.Email ?? "";
                }
                else
                {
                    result.HasError = true;
                    result.Messages?.Add("something wrong");
                }

                



                result.Data.Id = obj.Id;


                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private string GenerateRandomAlphanumericString(Random random)
        {
            string characters = Guid.NewGuid().ToString().Replace("-", "");
            StringBuilder result = new StringBuilder(9);

            for (int i = 0; i < 9; i++)
            {
                result.Append(characters[random.Next(characters.Length)]);
            }

            return result.ToString();
        }
        public async Task ProcessMessage(MessageGenerateCommand command)
        {
            await _mediator.Send(command);
        }

    }

}
