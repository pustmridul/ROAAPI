using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Booking.Models;
using MemApp.Application.Models;
using MemApp.Domain.Entities.ser;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Sales.Booking.Command
{
    public class VenueBlockedSelectedDateCommand : IRequest<Result>
    {
        public int VenueId { get; set; }
        public string? VeneuTitle { get; set; }
        public List<DateTime> SelectedDates { get; set; }
    }

    public class VenueBlockedSelectedDateCommandHandler : IRequestHandler<VenueBlockedSelectedDateCommand, Result>
    {
        private readonly IMemDbContext _context;

        public VenueBlockedSelectedDateCommandHandler(
            IMemDbContext context
            )
        {
            _context = context;
          
        }
        public async Task<Result> Handle(VenueBlockedSelectedDateCommand request, CancellationToken cancellation)
        {      
            try
            {
                var result = new Result();

                foreach( var date in request.SelectedDates )
                {
                    var obj = await _context.VenueBlockedSetups.SingleOrDefaultAsync(q => q.BlockedDate == date, cancellation);
                    if (obj == null)
                    {
                        obj = new VenueBlockedSetup()
                        {
                            BlockedDate = date,
                            IsActive = true,
                            MonthName = date.ToString("MMMM"),
                            DayName = date.ToString("dddd"),
                            YearName = date.ToString("yyyy"),
                            VenueId = request.VenueId,
                            VenueTitle=request.VeneuTitle
                        };
                        _context.VenueBlockedSetups.Add(obj);
                    }
                }
                await _context.SaveChangesAsync(cancellation);
                result.HasError = false;
                result.Messages.Add("Save success");

                return result;
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }



        }
    }
}
