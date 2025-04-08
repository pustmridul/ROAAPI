using Hangfire;
using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Booking.Models;
using MemApp.Application.Mem.TopUps.Command;
using MemApp.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Sales.Booking.Command
{
    public class CreateVenueBookingPaymentCommand : IRequest<Result>
    {
        public VenueBookingPaymentReq Model { get; set; } = new VenueBookingPaymentReq();
    }

    public class CreateVenueBookingPaymentCommandHandler : IRequestHandler<CreateVenueBookingPaymentCommand, Result>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMemLedgerService _memLedgerService;
        private readonly IBroadcastHandler _broadcastHandler;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IMediator _mediator;
        public CreateVenueBookingPaymentCommandHandler(IMemDbContext context, IPermissionHandler permissionHandler,
            ICurrentUserService currentUserService, IMemLedgerService memLedgerService, IBroadcastHandler broadcastHandler,
            IBackgroundJobClient backgroundJobClient, IMediator mediator)
        {
            _context = context;
            _permissionHandler = permissionHandler;
            _currentUserService = currentUserService;
            _memLedgerService = memLedgerService;
            _broadcastHandler = broadcastHandler;
            _backgroundJobClient = backgroundJobClient;
            _mediator = mediator;
        }
        public async Task<Result> Handle(CreateVenueBookingPaymentCommand request, CancellationToken cancellation)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }

            var result = new VenueBookingVm();


            if (await _permissionHandler.IsTempMember())
            {
                result.HasError = true;
                result.Messages?.Add("You have no permission to access, please contact with authority.");
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
                    var obj = await _context.VenueBookings
                         .Include(i => i.VenueBookingDetails)
                         .SingleOrDefaultAsync(q => q.Id == request.Model.VenueBookingId, cancellation);

                    if (obj == null)
                    {
                        result.HasError = true;
                        result.Messages?.Add("Data not found");
                        return result;

                    }

                    //var currentBalance = await _memLedgerService.GetCurrentBalance(obj.MemberId.ToString());

                    //if (currentBalance < request.Model.PaymentAmount)
                    //{
                    //    result.HasError = false;
                    //    result.Messages?.Add("You have not enough balance ! Top-up first ");
                    //    return result;
                    //}

                    obj.PaymentAmount += request.Model.PaymentAmount;
                    obj.PaymentDate = DateTime.Parse(request.Model.PaymentDate);

                    if (obj.TotalAmount > obj.PaymentAmount)
                    {
                        obj.BookingStatus = "Pending";
                    }
                    else
                    {
                        obj.BookingStatus = "Success";
                    }

                    if (await _context.SaveChangesAsync(cancellation) > 0)
                    {
                        result.HasError = false;
                        result.Messages?.Add("Venue Booking Payment Successful");

                        if (request.Model.PaymentAmount > 0)
                        {
                            var memberObj = await _context.RegisterMembers.Select(s => new
                            {
                                s.Id,
                                s.MembershipNo,
                                s.CardNo,
                                s.MemberId,
                                s.Phone,
                                s.Email
                            }).FirstOrDefaultAsync(q => q.Id == obj.MemberId, cancellation);
                            var venuTitle = obj?.VenueBookingDetails?.FirstOrDefault()?.VenueTitle ?? "";
                            var memLedger = new MemLedgerVm()
                            {
                                ReferenceId = obj.BookedNo,
                                Amount = (-1) * request.Model.PaymentAmount,
                                Dates = obj?.PaymentDate,
                                PrvCusID = obj?.MemberId.ToString() ?? "",
                                TOPUPID = "",
                                UpdateBy = _currentUserService.Username,
                                UpdateDate = DateTime.Now,
                                DatesTime = DateTime.Now,
                                Notes = "VenueBooking :  bookedId : " + obj?.Id,
                                PayType = "",
                                TransactionFrom = _currentUserService.AppId,
                                TransactionType = "VENUEBOOKING",
                                Description = "MemberShip No : " + memberObj?.MembershipNo + ", Card No : " + memberObj?.CardNo + ",Venue Title : " + venuTitle + ", Booked No : " + obj.BookedNo + "Booking Date :" + obj.BookedDate + ", Amount : " + obj.Amount + ", VAT : " + obj.VatAmount + ", Service Charge : " + obj.ServiceAmount,
                            };

                            await _memLedgerService.CreateMemLedger(memLedger);
                            if (memberObj != null)
                            {
                                var curentBalance = await _memLedgerService.GetCurrentBalance(memberObj.Id.ToString());
                                string message = "";
                                string subject = "";

                                if (memberObj.Phone != null)
                                {

                                    message = "Dear CCCL Member, Tk. " + Math.Round(request.Model.PaymentAmount, 2) + " has been deducted from your wallet, current balance is Tk. " + Math.Round(curentBalance, 2) + ", Thanks.";

                                    _backgroundJobClient.Enqueue<IBroadcastHandler>(e => e.SendSms(memberObj.Phone, message, "English",null,null));
                                    message = "";
                                }
                                if (memberObj.Email != null)
                                {

                                    message = "Dear CCCL Member, Tk. " + Math.Round(request.Model.PaymentAmount, 2) + " has been deducted from your wallet, current balance is Tk. " + Math.Round(curentBalance, 2) + ", Thanks.";

                                    subject = "Venue Booking (Cadet College Club Ltd) ";
                                    _backgroundJobClient.Enqueue<IBroadcastHandler>(e => e.SendEmail(memberObj.Email, subject, message, null, null));
                                    message = "";
                                    subject = "";
                                }
                            }
                        }
                    }
                    await _context.SaveChangesAsync(cancellation);
                    await transaction.CommitAsync(cancellation);
                    return result;
                }

                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellation);
                    throw new Exception(ex.Message);
                }
            }

        }
    }
}
