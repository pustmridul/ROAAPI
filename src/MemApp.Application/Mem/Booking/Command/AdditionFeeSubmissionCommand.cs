using Hangfire;
using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Booking.Models;
using MemApp.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Booking.Command
{
    public class AdditionFeeSubmissionCommand : IRequest<VenueBookingVm>
    {
        public VenueBookingReq Model = new VenueBookingReq();
    }

    public class AdditionFeeSubmissionCommandHandler : IRequestHandler<AdditionFeeSubmissionCommand, VenueBookingVm>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMemLedgerService _memLedgerService;
        private readonly IBroadcastHandler _broadcastHandler;
        private readonly IBackgroundJobClient _backgroundJobClient;
        public AdditionFeeSubmissionCommandHandler(
            IMemDbContext context,
            IPermissionHandler permissionHandler,
            IUserLogService userLogService,
            ICurrentUserService currentUserService,
            IMemLedgerService memLedgerService,
            IBroadcastHandler broadcastHandler,
            IBackgroundJobClient backgroundJobClient)
        {
            _context = context;
            _permissionHandler = permissionHandler;
            _currentUserService = currentUserService;
            _memLedgerService = memLedgerService;
            _broadcastHandler = broadcastHandler;
            _backgroundJobClient = backgroundJobClient;
        }
        public async Task<VenueBookingVm> Handle(AdditionFeeSubmissionCommand request, CancellationToken cancellation)
        {

            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            var result = new VenueBookingVm();


            var venueBooking = await _context.VenueBookings.Where(c => c.Id == request.Model.Id).SingleOrDefaultAsync();
            if (venueBooking != null)
            {
                venueBooking.AdditionalFee = request.Model.AdditionalFee;
                if (request.Model.AdditionalFeeNote != null)
                {
                    venueBooking.AdditionalFeeNote = request.Model.AdditionalFeeNote;
                }

                await _context.SaveChangesAsync(cancellation);
            }

            var memberObj = await _context.RegisterMembers.Select(s => new
            {
                s.Id,
                s.MembershipNo,
                s.CardNo,
                s.MemberId,
                s.Phone,
                s.Email
            }).FirstOrDefaultAsync(q => q.Id == venueBooking.MemberId, cancellation);

            var venuTitle = venueBooking?.VenueBookingDetails?.FirstOrDefault()?.VenueTitle ?? "";

            var memLedger = new MemLedgerVm()
            {

                TransactionType = "VENUEBOOKING",
                TransactionFrom = _currentUserService.AppId,
                ReferenceId = venueBooking.BookedNo,
                Amount = (-1) * venueBooking.PaymentAmount,
                Dates = venueBooking.PaymentDate,
                PrvCusID = venueBooking.MemberId.ToString(),
                TOPUPID = "",
                UpdateBy = _currentUserService.Username,
                UpdateDate = DateTime.Now,
                DatesTime = DateTime.Now,
                Notes = "VenueBooking :  bookedId : " + venueBooking.Id,
                PayType = "",

                Description = "MemberShip No : " + memberObj?.MembershipNo + ", Card No : " + memberObj?.CardNo + ",Venue Title : " + venuTitle + ", Booked No : " + venueBooking.BookedNo + "Booking Date :" + venueBooking.BookedDate + ", Amount : " + venueBooking.Amount + ", VAT : " + venueBooking.VatAmount + ", Service Charge : " + venueBooking.ServiceAmount + ", AdditionalFee : " + request.Model.AdditionalFee + (request.Model.AdditionalFeeNote != null ? ", AdditionalFeeNote: " + request.Model.AdditionalFeeNote : ""),

            };

            await _memLedgerService.CreateMemLedger(memLedger);

            return result;
        }
    }
}
