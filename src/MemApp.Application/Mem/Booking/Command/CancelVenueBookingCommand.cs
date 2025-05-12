using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Booking.Models;
using MemApp.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Sales.Booking.Command
{
    public class CancelVenueBookingCommand : IRequest<Result>
    {
        public VenueBookingReq Model { get; set; } = new VenueBookingReq();
    }

    public class CancelVenueBookingCommandHandler : IRequestHandler<CancelVenueBookingCommand, Result>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        ICurrentUserService _currentUserService;
        IMemLedgerService _memLedgerService;


        public CancelVenueBookingCommandHandler(
            IMemDbContext context,
            IMediator mediator,
            ICurrentUserService currentUserService,
            IMemLedgerService memLedgerService
            )
        {
            _context = context;
            _mediator = mediator;
            _currentUserService = currentUserService;
            _memLedgerService = memLedgerService;
        }
        public async Task<Result> Handle(CancelVenueBookingCommand request, CancellationToken cancellation)
        {
            var result = new Result() ;
            
            
            try
            {             
                decimal refundAmount = 0;
                var venueBooking = await _context.VenueBookings
               .Include(c => c.VenueBookingDetails)
               .Where(c => c.Id == request.Model.Id)
               .FirstOrDefaultAsync(cancellation);



                if (venueBooking != null)
                {
                    if (venueBooking.PaymentAmount == 0)
                    {
                       

                    }
                    var selectedDetailIdsToCancel = request.Model.VenueBookingDetailReqs.Select(c => c.Id).ToList();

                    foreach (var venueBookingDetail in venueBooking.VenueBookingDetails.Where(c => selectedDetailIdsToCancel.Contains(c.Id)))
                    {
                        // Assign a value to the specific property (SomeProperty) of each element
                        venueBookingDetail.SaleStatus = "Cancel";
                        venueBookingDetail.IsCancelled = true;
                        venueBookingDetail.CancellationNote = request.Model.VenueBookingDetailReqs.Where(c => c.Id == venueBookingDetail.Id).FirstOrDefault()?.Note;
                        venueBookingDetail.RefundAmount = request.Model.VenueBookingDetailReqs.Where(c => c.Id == venueBookingDetail.Id).FirstOrDefault()?.RefundAmount;
                        refundAmount += venueBookingDetail.RefundAmount??0;
                    }
                    await _context.SaveChangesAsync(cancellation);
                    if (venueBooking.VenueBookingDetails.All(c => c.SaleStatus?.Trim() == "Cancel"))
                    {
                        venueBooking.BookingStatus = "Cancel";

                        await _context.SaveChangesAsync(cancellation);
                    }
                    var note = "";
                    //Create ledger
                    foreach (var item in request.Model.VenueBookingDetailReqs)
                    {
                        if (!string.IsNullOrEmpty(item.Note))
                        {
                            note += item.Note + ". ";
                        }
                    }

                    string bookedDate = DateTime.ParseExact(
                    request.Model.BookedDate,
                    "yyyy-MM-ddTHH:mm:ss.fffZ",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.AdjustToUniversal
                ).ToString("dd-MM-yyyy");

                    string preFix = "R";
                    var refundNo = "";
                    var max = _context.MemLedgers.Where(q => q.RefundId!.StartsWith(preFix))
                        .Select(s => s.RefundId!.Replace(preFix, "")).DefaultIfEmpty().Max();

                    if (string.IsNullOrEmpty(max))
                    {
                        refundNo = preFix + "0000001";
                    }
                    else
                    {
                        refundNo = preFix + (Convert.ToInt32(max) + 1).ToString("0000000");
                    }

                    var memLedger = new MemLedgerVm()
                    {
                        
                        TransactionType = "VENUEBOOKING",
                        TransactionFrom = _currentUserService.AppId,
                        ReferenceId= venueBooking.BookedNo,
                        RefundId = refundNo,
                        Amount = refundAmount,
                        Dates = DateTime.Now.Date,
                        PrvCusID = request.Model.MemberId.ToString(),
                        TOPUPID = null,
                        UpdateBy = _currentUserService.Username,
                        UpdateDate = DateTime.Now,
                        DatesTime = DateTime.Now,
                        Notes = "Venue Booking Price Amount : " + request.Model.Amount + ", Vat Amount : " + request.Model.VatAmount + ", Service Charge Amount : " + request.Model.ServiceAmount + "Refund Amount :" + request.Model.VenueBookingDetailReqs.Sum(c => c.TicketPrice),
                        PayType = "",
                        Description = "Venue Booking " + request.Model.BookedNo + " " + "Booking Date :" + bookedDate + " " + "Refund Amount :" + request.Model.VenueBookingDetailReqs.Sum(c => c.TicketPrice) + (string.IsNullOrEmpty(note) ? "" : ". Cancellation Note: " + note)
                    };

                     result.Succeeded = await _memLedgerService.CreateMemLedger(memLedger);

                    return result;
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex) { 
             throw new Exception( ex.Message );
            }
           
        }
    }
}
