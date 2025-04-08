using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Booking.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Booking.Queries
{
    public class GetBookingByDateQuery : IRequest<BookingInfoVm>
    {
        public int VenueId { get; set; }
        public int AvailabilityId { get; set; }
        public DateTime BookedDate { get; set; }

    }


    public class GetBookingByDateQueryHandler : IRequestHandler<GetBookingByDateQuery, BookingInfoVm>
    {
        private readonly IMemDbContext _context;
        public GetBookingByDateQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<BookingInfoVm> Handle(GetBookingByDateQuery request, CancellationToken cancellationToken)
        {
            var result = new BookingInfoVm();
            try
            {         
                var data = await _context.VenueBookingDetails.Where(q=>q.IsActive && q.IsCancelled==false)
                    .Include(i => i.VenueBooking).ThenInclude(i => i.Member)
                         .FirstOrDefaultAsync(q => q.IsActive && q.VenueId== request.VenueId
                         && q.BookingDate.Date == request.BookedDate.Date
                         && q.AvailabilityId == request.AvailabilityId, cancellationToken);
                if (data == null)
                {
                   var  dataS = await _context.VenueBlockedSetups
                        .Where(q => q.IsActive && q.BlockedDate.Date == request.BookedDate.Date 
                        && q.VenueId==request.VenueId && (q.AvailabilityId>0? q.AvailabilityId== request.AvailabilityId: true))
                        .ToListAsync(cancellationToken);

                    if (dataS == null)
                    {
                        result.HasError = true;
                        result.Messages.Add("Data Not Found");
                    }
                    result.HasError = false;

                    result.Data = new BookingInfoReq
                    {
                        MemberName ="Blocked by System Admin",
                        MemberShipNo = "",
                        PhoneNo =  ""
                    };
                }
                else
                {
                    result.HasError = false;

                    result.Data = new BookingInfoReq
                    {
                        MemberName=data.VenueBooking.Member.FullName,
                        MemberShipNo=data.VenueBooking.MemberShipNo ?? "",
                        PhoneNo=data.VenueBooking.Member.Phone??""
                    };

                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Messages.Add(ex.Message);
            }
            return result;  
        }
    }
}
