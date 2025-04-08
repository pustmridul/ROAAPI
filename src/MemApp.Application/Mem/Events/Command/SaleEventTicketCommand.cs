using Hangfire;
using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Events.Models;
using MemApp.Application.Mem.TopUps.Command;
using MemApp.Application.MessageInboxs.Commands.MessageGenerateCommand;
using MemApp.Application.MessageInboxs.Models;
using MemApp.Domain.Entities.Communication;
using MemApp.Domain.Entities.mem;
using MemApp.Domain.Entities.ser;
using MemApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace MemApp.Application.Mem.Events.Command
{
    public class CreateSaleEventTicketCommand : IRequest<SaleEventTicketVm>
    {
        public SaleEventTicketReq Model { get; set; } = new SaleEventTicketReq();
    }

    public class CreateSaleEventTicketCommandHandler : IRequestHandler<CreateSaleEventTicketCommand, SaleEventTicketVm>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMemLedgerService _memLedgerService;
        private readonly IMediator _mediator;
        private readonly IBackgroundJobClientV2 _backgroundJobClientV2;
    
        public CreateSaleEventTicketCommandHandler(
            IMemDbContext context,
            IPermissionHandler permissionHandler,
            ICurrentUserService currentUserService,
            IMemLedgerService memLedgerService,
            IMediator mediator,
            IBackgroundJobClientV2 backgroundJobClientV2)
        {
            _context = context;
            _permissionHandler = permissionHandler;
            _currentUserService = currentUserService;
            _memLedgerService = memLedgerService;
            _mediator = mediator;
            _backgroundJobClientV2 = backgroundJobClientV2;
        }
        public async Task<SaleEventTicketVm> Handle(CreateSaleEventTicketCommand request, CancellationToken cancellation)
        {
            var result = new SaleEventTicketVm();
            bool newObj = false;

            string eventTitle = "";
            if (await _permissionHandler.IsTempMember())
            {
                result.HasError = true;
                result.Messages.Add("You have no permission to access, please contact with authority.");
                return result;
            }
            if (request.Model.TopUpReq is not null)
            {
                await _mediator.Send(new CreateTopUpCommand() { Model = request.Model.TopUpReq });
            }


            using (var transaction = await _context.BeginTransactionAsync(cancellation))
            {

                try
                {
                    var curentBalance = await _memLedgerService.GetCurrentBalance(request.Model.MemberId.ToString());

                    if (curentBalance < 0)
                    {
                        if(request.Model.TopUpReq is not null)
                        {
                            if(request.Model.PaymentAmount < request.Model.TopUpReq.TotalAmount)
                            {
                                result.HasError= true;
                                result.Messages.Add("Ticket price is more then your Top-up?");
                                return result;
                            }
                        }
                    }

                    if (request.Model.InvoiceDate?.Date < DateTime.Now.Date)
                    {
                        result.HasError = true;
                        result.Messages.Add("Invoice Date is not valid");
                        return result;
                    }

                    var serviceTicket = await _context.ServiceTickets.SingleOrDefaultAsync(q => q.Id == request.Model.SaleEventTicketDetailReqs.First().EventId, cancellation);
                    if (serviceTicket != null)
                    {
                        if (serviceTicket.EndDate?.Date < DateTime.Now.Date)
                        {
                            result.HasError = true;
                            result.Messages.Add("Event " + request.Model.SaleEventTicketDetailReqs.First().EventTitle + ", Ticket Selling Date is over !!!");
                            return result;
                        }
                    }

                    bool hasBuy = false;
                    foreach (var d in request.Model.SaleEventTicketDetailReqs)
                    {

                        if (d.AreaLayoutId > 0)
                        {
                            var buy = await _context.SaleEventTicketDetails
                               .Where(q => q.NoofChair == d.NoofChair && q.EventId == d.EventId && q.IsActive && q.TableId == d.TableId && q.SaleStatus != "Cancel")
                               .ToListAsync();
                            if (buy.Any())
                            {
                                hasBuy = true;
                                result.HasError = true;
                                result.Messages.Add("Event :" + d.EventTitle + "Layout : " + d.AreaLayoutTitle + "Table : " + d.TableTitle + "Chair : " + d.NoofChair);
                                result.Messages.Add("Already Buy Someone");

                            }
                        }

                    }
                    if (hasBuy)
                    {
                        return result;
                    }


                    var obj = await _context.SaleEventTickets
                        .Include(i => i.SaleEventTicketDetails)
                    .SingleOrDefaultAsync(q => q.Id == request.Model.Id);

                    if (obj == null)
                    {
                        newObj = true;

                        obj = new SaleEventTicket()
                        {
                            MemberId = request.Model.MemberId,
                            InvoiceDate = DateTime.Now,
                            MemberShipNo = request.Model.MemberShipNo,
                            IsActive = true
                        };
                        _context.SaleEventTickets.Add(obj);

                        result.HasError = false;
                        result.Messages.Add("Create new Sale Event Ticket");

                    }

                    if (newObj)
                    {
                        DateTime invoiceDateTime = DateTime.Now;
                        string preFix = "E" + invoiceDateTime.ToString("yyMMdd");

                       

                        var max = _context.SaleEventTickets.Where(q => q.InvoiceNo.StartsWith(preFix))
                            .Select(s => s.InvoiceNo.Replace(preFix, "")).DefaultIfEmpty().Max();

                        if (string.IsNullOrEmpty(max))
                        {
                            obj.InvoiceNo = preFix + "0001";
                        }
                        else
                        {
                            obj.InvoiceNo = preFix + (Convert.ToInt32(max) + 1).ToString("0000");
                        }

                    }

                    obj.PaymentAmount = request.Model.PaymentAmount;
                    obj.Amount = request.Model.Amount;
                    obj.VatAmount = request.Model.VatAmount;
                    obj.ServiceAmount = request.Model.ServiceAmount;
                    obj.PaymentDate = request.Model.PaymentDate ?? DateTime.Now;
                    obj.TotalAmount = request.Model.Amount + request.Model.VatAmount + request.Model.ServiceAmount;
                    obj.SaleStatus = request.Model.SaleStatus ?? "Confirm";
                    obj.OrderFrom = request.Model.OrderFrom;

                    await _context.SaveChangesAsync(cancellation);
                    // details save
                    foreach (var d in request.Model.SaleEventTicketDetailReqs)
                    {



                        var objDetail = await _context.SaleEventTicketDetails.SingleOrDefaultAsync(i => i.Id == d.Id);


                        if (objDetail == null)
                        {
                            objDetail = new SaleEventTicketDetail()
                            {
                                EventId = d.EventId,
                                EventTokens = d.EventTokens,
                                EventTitle = d.EventTitle,
                                SaleEventId = obj.Id,
                                AreaLayoutId = d.AreaLayoutId,
                                AreaLayoutTitle = d.AreaLayoutTitle,
                                TableId = d.TableId,
                                TableTitle = d.TableTitle,
                                NoofChair = d.NoofChair,
                                IsActive = true,
                                TicketCodeNo = GenerateRandomAlphanumericString(new Random()),
                                TicketCriteria = d.TicketCriteria,
                                TicketCriteriaId = d.TicketCriteriaId,
                                TicketPrice = d.TicketPrice,
                                TicketText = d.TicketText,
                                VatPercent = d.VatPercentage ?? 0,
                                VatAmount = d.VatAmount,
                                ServiceChargeAmount = d.ServiceChargeAmount,
                                SaleStatus = "Confirm"

                            };
                            _context.SaleEventTicketDetails.Add(objDetail);
                        }
                        eventTitle = d.EventTitle ?? "";
                    }



                    var memberObj = await _context.RegisterMembers.Select(s => new
                    {
                        s.Id,
                        s.MembershipNo,
                        s.CardNo,
                        s.MemberId,
                        s.Phone,
                        s.Email,
                        s.FullName
                    }).FirstOrDefaultAsync(q => q.Id == obj.MemberId, cancellation);
                    if (obj.TotalAmount > 0)
                    {
                        var memLedger = new MemLedger()
                        {
                            ReferenceId = obj.InvoiceNo,
                            Amount = (-1) * obj.TotalAmount,
                            Dates = obj.PaymentDate,
                            PrvCusID = obj.MemberId.ToString(),
                            TOPUPID = "",
                            UpdateBy = _currentUserService.Username,
                            UpdateDate = DateTime.Now,
                            DatesTime = DateTime.Now,
                            Notes = "EventSale : SaleId :" + obj.Id,
                            PayType = "",
                            TransactionType = "EVENT",
                            TransactionFrom = _currentUserService.AppId,
                            Description = "MemberShip No : " + memberObj?.MembershipNo + ", CardNo : " + memberObj?.CardNo + ", Event : " + eventTitle + ",Invoice : " + obj.InvoiceNo + ",Date : " + obj.InvoiceDate + ",Ticket Amount : " + obj.PaymentAmount + " ,Amount : " + obj.Amount + ", VAT : " + obj.VatAmount + ", Service Charge : " + obj.ServiceAmount,

                        };
                        _context.MemLedgers.Add(memLedger);
                    }



                    if (memberObj != null)
                    {
                        //Admin Notification saved
                        var adminNotification = new AdminNotification
                        {
                            Message = $"A event ticket has been sold! Member: {memberObj.FullName}, Event: {obj.SaleEventTicketDetails.First().EventTitle}, Event Date: {obj.SaleEventTicketDetails.First().Event.EventDate?.ToString("dd MMM yyyy")}",
                            TypeId = MessageInboxTypeEnum.EventTicketSale,
                            IsRead = false,
                        }; 
                        _context.AdminNotifications.Add(adminNotification);

                        //Admin mail send
                        var notificationEmail = await _context.NotificationEmails.FirstOrDefaultAsync();
                        _backgroundJobClientV2.Enqueue<IBroadcastHandler>(e => e.SendEmail(notificationEmail.EventSaleNotificationEmail, "Event Ticket Sale", adminNotification.Message,null,null));



                        curentBalance = await _memLedgerService.GetCurrentBalance(memberObj.Id.ToString());
                        string message = "";
                        if (memberObj.Phone != null)
                        {
                            message = "Dear CCCL Member, Tk. " + Math.Round(obj.TotalAmount, 2) + " has been deducted from your wallet, current balance is Tk. " + Math.Round(curentBalance, 2) + ", Thanks.";


                            result.Data.SmsText = message;
                            result.Data.PhoneNo = memberObj.Phone;
                           
                          
                         
                        }

                        if ( request.Model.Id == 0)
                        {
                            var messageObj = new MessageInboxCreateDto
                            {
                                MemberId = obj.MemberId,
                                Message = message,
                                TypeId = MessageInboxTypeEnum.EventTicketSale,
                                IsRead = false,
                                IsAllMember = false,

                            };
                            _backgroundJobClientV2.Enqueue(() => ProcessMessage(new MessageGenerateCommand() { Model = messageObj }));
                        }
                    }

                    await _context.SaveChangesAsync(cancellation);
                    await transaction.CommitAsync(cancellation);



                    result.Data.Id = obj.Id;
                    result.Data.InvoiceDate = obj.InvoiceDate;
                    result.Data.InvoiceNo = obj.InvoiceNo;
                    result.Data.MemberId = obj.MemberId;
                    result.Data.MemberEmail = memberObj?.Email ?? "";

                    result.Data.Id = obj == null ? 0 : obj.Id;
                    return result;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellation);
                    throw new Exception(ex.Message);
                }
            }


        }
        public async Task ProcessMessage(MessageGenerateCommand command)
        {
            await _mediator.Send(command);
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


        private static async Task<byte[]> GetPdfBytesFromApiAsync(string apiUrl)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
                else
                {
                    throw new Exception($"API request failed with status code: {response.StatusCode}");
                }
            }
        }

    }
}
