using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Booking.Models;
using MemApp.Application.Models;
using MemApp.Domain.Entities.ser;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Sales.Booking.Command
{
    public class VenueBlockedMultipleCommand : IRequest<Result>
    {
        public VenueBlockedMultipleReq Model { get; set; } = new VenueBlockedMultipleReq();
    }

    public class VenueBlockedMultipleCommandHandler : IRequestHandler<VenueBlockedMultipleCommand, Result>
    {
        private readonly IMemDbContext _context;

        public VenueBlockedMultipleCommandHandler(
            IMemDbContext context
            )
        {
            _context = context;
          
        }
        public async Task<Result> Handle(VenueBlockedMultipleCommand request, CancellationToken cancellation)
        {      
            try
            {
                var result = new Result();

              
                    if (request.Model.BlockedDate != null)
                    {
                        //var obj = await _context.VenueBlockedSetups.SingleOrDefaultAsync(q => q.BlockedDate == request.Model.BlockedDate
                        //&& q.VenueIdrequest.Model.VenueIds.c, cancellation);
                        //if (obj == null)
                        //{
                        //    obj = new VenueBlockedMultiple()
                        //    {
                        //        BlockedDate = request.Model.BlockedDate.Value.Date,
                        //        IsActive = true,
                        //        MonthName = request.Model.BlockedDate==null ? DateTime.Now.ToString("MMMM") : request.Model.BlockedDate.Value.ToString("MMMM"),
                        //        DayName = request.Model.BlockedDate == null ? DateTime.Now.ToString("dddd") : request.Model.BlockedDate.Value.ToString("dddd"),
                        //        YearName = request.Model.BlockedDate==null ? DateTime.Now.ToString("yyyy") : request.Model.BlockedDate.Value.ToString("yyyy"),
                        //        VenueId=request.Model.VenueId??0,
                        //        VenueTitle=request.Model.VenueTitle??""
                        //    };
                        //    _context.VenueBlockedMultiples.Add(obj);
                        //    await _context.SaveChangesAsync(cancellation);
                        //    result.HasError = false;
                        //    result.Messages.Add("Venue Blocked");
                        //}
                    }
                  
                


                return result;
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }



        }
    }
}
