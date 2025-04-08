using FirebaseAdmin.Messaging;
using Hangfire;
using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Booking.Models;
using MemApp.Application.MessageInboxs.Commands.MessageGenerateCommand;
using MemApp.Application.MessageInboxs.Models;
using MemApp.Application.Models;
using MemApp.Application.Models.DTOs;
using MemApp.Domain.Entities.com;
using MemApp.Domain.Entities.Communication;
using MemApp.Domain.Entities.mem;
using MemApp.Domain.Entities.ser;
using MemApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Sales.Booking.Command
{
    public class CreateVenueBookingCommand : IRequest<VenueBookingVm>
    {
        public VenueBookingReq Model = new VenueBookingReq();
    }

    public class CreateVenueBookingCommandHandler : IRequestHandler<CreateVenueBookingCommand, VenueBookingVm>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        private readonly ICurrentUserService _currentUserService; 
        private readonly IMemLedgerService _memLedgerService;
        private readonly IBroadcastHandler _broadcastHandler;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IMediator _mediator;
        private readonly IBackgroundJobClientV2 _backgroundJobClientV2;
        public CreateVenueBookingCommandHandler(
            IMemDbContext context,
            IPermissionHandler permissionHandler,
            IUserLogService userLogService,
            ICurrentUserService currentUserService,
            IMemLedgerService memLedgerService,
            IBroadcastHandler broadcastHandler,
            IBackgroundJobClient backgroundJobClient,
            IMediator mediator,
            IBackgroundJobClientV2 backgroundJobClientV2
            )
        {
            _context = context;
            _permissionHandler = permissionHandler;
            _currentUserService = currentUserService;
            _memLedgerService = memLedgerService;
            _broadcastHandler = broadcastHandler;
            _backgroundJobClient= backgroundJobClient;
            _mediator = mediator;
            _backgroundJobClientV2 = backgroundJobClientV2;
        }
        public async Task<VenueBookingVm> Handle(CreateVenueBookingCommand request, CancellationToken cancellation)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            var result = new VenueBookingVm();
            if (!request.Model.IsTramsAndCondition)
            {
                result.HasError = true;
                result.Messages?.Add("Accept trams and condition");
                return result;
            }
            bool newObj = false;
            string venuTitle = "";          
            if (await _permissionHandler.IsTempMember())
            {
                result.HasError = true;
                result.Messages?.Add("You have no permission to access, please contact with authority.");
                return result;
            }
            try
            {
                List<VenueAddOnsItemDetail> VenueAddOnsItemDetails = new List<VenueAddOnsItemDetail>();
                if (DateTime.Parse(request.Model.BookedDate).Date < DateTime.Now.Date)
                {
                    result.HasError = true;
                    result.Messages?.Add("Invoice Date is not valid");
                    return result;
                }
                var obj = await _context.VenueBookings
                    .Include(i => i.VenueBookingDetails)

                .SingleOrDefaultAsync(q => q.Id == request.Model.Id, cancellation);

                if (obj == null)
                {
                    newObj = true;

                    obj = new VenueBooking()
                    {
                        MemberId = request.Model.MemberId,
                        BookedDate = DateTime.Parse(request.Model.BookedDate),
                        MemberShipNo = request.Model.MemberShipNo,
                        IsActive = true,
                        IsTramsAndCondition = request.Model.IsTramsAndCondition
                    };
                    _context.VenueBookings.Add(obj);

                    result.HasError = false;
                    result.Messages?.Add("Create new Venue Booking");

                }

                if (newObj)
                {
                    string preFix = "V" + DateTime.Now.ToString("yyMMdd");

                    var max = _context.VenueBookings.Where(q => q.BookedNo.StartsWith(preFix))
                        .Select(s => s.BookedNo.Replace(preFix, "")).DefaultIfEmpty().Max();

                    if (string.IsNullOrEmpty(max))
                    {
                        obj.BookedNo = preFix + "0001";
                    }
                    else
                    {
                        obj.BookedNo = preFix + (Convert.ToInt32(max) + 1).ToString("0000");
                    }

                }
                obj.BookingCriteria = request.Model.BookingCriteria;
                obj.BookingCriteriaId = request.Model.BookingCriteriaId;
                obj.BookingPrice = request.Model.BookingPrice;
                obj.OrderFrom = request.Model.OrderFrom;
                obj.PaymentAmount = request.Model.PaymentAmount;
                obj.DiscountAmount = request.Model.DiscountAmount;
                obj.PaymentDate = DateTime.Parse(request.Model.PaymentDate);
                obj.Amount = request.Model.Amount;
                obj.VatAmount = request.Model.VatAmount;
                obj.ServiceAmount = request.Model.ServiceAmount;
                obj.TotalAmount = request.Model.TotalAmount;
                obj.BookingStatus = request.Model.BookingStatus;
                obj.RefName = request.Model.RefName;
                obj.RefRelation = request.Model.RefRelation;
                obj.Note = request.Model.Note;
                obj.BookingPurpose = request.Model.BookingPurpose;
                obj.RefPhoneNo = request.Model.RefPhoneNo;

                await _context.SaveChangesAsync(cancellation);
               

                foreach (var d in request.Model.VenueBookingDetailReqs)
                {
                    var objDetail = await _context.VenueBookingDetails.SingleOrDefaultAsync(i => i.Id == d.Id);
                    if (objDetail == null)
                    {
                        objDetail = new VenueBookingDetail()
                        {
                            VenueId = d.VenueId,
                            BookingDate = DateTime.Parse(request.Model.BookedDate),
                            VenueTitle = d.VenueTitle,
                            BookingId = obj.Id,
                            IsActive = true,
                            AvailabilityId = d.AvailabilityId,
                            ServiceChargeAmount = ((request.Model.BookingPrice* request.Model.ServicePercent)/100)- request.Model.DiscountAmount,
                            VatAmount = ((request.Model.BookingPrice * request.Model.VatPercentage) / 100) - request.Model.DiscountAmount,
                            VatPercent= request.Model.VatPercentage ??0,


                        };
                        _context.VenueBookingDetails.Add(objDetail);
                        await _context.SaveChangesAsync(cancellation);
                        foreach(var item in d.VenueBookingAddOnsItemReqs)
                        {
                            var addon = new VenueAddOnsItemDetail
                            {

                                AddOnsItemId = item.AddOnsItemId,
                                BookingId = obj.Id,
                                BookingDetailId = objDetail.Id,
                                Title = item.Title,
                                Description = item.Description,
                                Price = item.Price,
                                PriceDate = item.PriceDate,
                                IsActive=true
                            };
                            VenueAddOnsItemDetails.Add(addon);

                        }
                      
                    }
                    venuTitle = obj?.VenueBookingDetails?.FirstOrDefault()?.VenueTitle ?? "";
                }
                if(VenueAddOnsItemDetails != null) { 
                _context.VenueAddOnsItemDetails.AddRange(VenueAddOnsItemDetails);
                }

                await _context.SaveChangesAsync(cancellation);

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
                var curentBalance = await _memLedgerService.GetCurrentBalance(memberObj.Id.ToString());
                if (obj?.PaymentAmount > 0)
                {
                   

                    var memLedger = new MemLedgerVm()
                    {
                        ReferenceId= obj.BookedNo,
                        Amount = (-1) * obj.PaymentAmount,
                        Dates = obj.PaymentDate,
                        PrvCusID = obj.MemberId.ToString(),
                        TOPUPID = "",
                        UpdateBy = _currentUserService.Username,
                        UpdateDate = DateTime.Now,
                        DatesTime = DateTime.Now,
                        Notes = "VenueBooking :  bookedId : " + obj.Id,
                        PayType = "",
                        TransactionType = "VENUEBOOKING",
                        TransactionFrom = _currentUserService.AppId,
                        Description = "MemberShip No : " + memberObj?.MembershipNo + ", Card No : " + memberObj?.CardNo + ",Venue Title : " + venuTitle + ", Booked No : " + obj.BookedNo + "Booking Date :" + obj.BookedDate + ", Amount : " + obj.Amount + ", VAT : " + obj.VatAmount + ", Service Charge : " + obj.ServiceAmount,

                    };

                    await _memLedgerService.CreateMemLedger(memLedger);

                    if (memberObj != null)
                    {
                        string message = "";
                        string subject = "";
                        if (memberObj.Phone != null)
                        {
                            message = "Dear CCCL Member, Tk. " + Math.Round(obj.PaymentAmount, 2) + " has been deducted from your wallet, current balance is Tk. " + Math.Round(curentBalance, 2) + ", Thanks.";

                            _backgroundJobClient.Enqueue<IBroadcastHandler>(e=>e.SendSms(memberObj.Phone, message, "English", null, null));
                            message = "";
                        }
                        if (memberObj.Email != null)
                        {
                            message = "Dear CCCL Member, Tk. " + Math.Round(obj.PaymentAmount, 2) + " has been deducted from your wallet, current balance is Tk. " + Math.Round(curentBalance, 2) + ", Thanks.";

                            subject = "Venue Booking (Cadet College Club Ltd) ";
                           _backgroundJobClient.Enqueue<IBroadcastHandler>(e=> e.SendEmail(memberObj.Email, subject, message,null, null));                         
                        }


                        
                    }
                }
                if (request.Model.Id == null || request.Model.Id == 0)
                {
                    //Admin Notification saved
                    var adminNotification = new AdminNotification
                    {
                        Message = $"A venue has been booked! Member: {memberObj.FullName}, Venue: {obj.VenueBookingDetails.First().VenueTitle}, Booking Date: {obj.VenueBookingDetails.First().BookingDate.ToString("dd MMM yyyy")}, Booked Date: {obj.BookedDate.ToString("dd MMM yyyy")}",
                        TypeId = MessageInboxTypeEnum.VenueBookingSale,
                        IsRead = false,
                    };
                    _context.AdminNotifications.Add(adminNotification);
                    await _context.SaveChangesAsync(cancellation);

                    //Admin mail send
                    var notificationEmail = await _context.NotificationEmails.FirstOrDefaultAsync();
                    _backgroundJobClientV2.Enqueue<IBroadcastHandler>(e => e.SendEmail(notificationEmail.VenueBookingNotificationEmail, "Venue Booking", adminNotification.Message, null, null));

                    var messageObj = new MessageInboxCreateDto
                    {
                        MemberId = obj.MemberId,
                        Message = "Dear CCCL Member, Tk. " + Math.Round(obj.PaymentAmount, 2) + " has been deducted from your wallet, current balance is Tk. " + Math.Round(curentBalance, 2) + ", Thanks.",
                        TypeId = MessageInboxTypeEnum.VenueBookingSale,
                        IsRead = false,
                        IsAllMember = false,

                    };
                    _backgroundJobClientV2.Enqueue(() => ProcessMessage(new MessageGenerateCommand() { Model = messageObj }));
                }

                if (memberObj != null)
                {
                    result.Data.MemberEmail = memberObj.Email;
                }
                result.Data.Id = obj==null?0: obj.Id;

                
            }


            

            catch (Exception ex)
            {
                result.HasError = true;
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
