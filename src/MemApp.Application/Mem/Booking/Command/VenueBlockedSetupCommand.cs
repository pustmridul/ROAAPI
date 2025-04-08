using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.MemServices.Models;
using MemApp.Application.Models;
using MemApp.Domain.Entities.ser;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Sales.Booking.Command
{
    public class VenueBlockedSetupCommand : IRequest<Result>
    {
        public List<VenueListWithAvailable> Models { get; set; } = new List<VenueListWithAvailable>();
    }

    public class VenueBlockedSetupCommandHandler : IRequestHandler<VenueBlockedSetupCommand, Result>
    {
        private readonly IMemDbContext _context;

        public VenueBlockedSetupCommandHandler(
            IMemDbContext context
            )
        {
            _context = context;
          
        }
        public async Task<Result> Handle(VenueBlockedSetupCommand request, CancellationToken cancellation)
        {      
            try
            {
                var result = new Result();

                //if (request.Model.IsThisYear)
                //{
                //    DateTime currentDate = DateTime.Now;
                //    DateTime startDate = new DateTime(currentDate.Year, 1, 1);
                //    DateTime endDate = new DateTime(currentDate.Year, 12, 31);

                //    for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                //    {
                //        if (request.Model.DayName != null)
                //        {
                //            if (date.DayOfWeek.ToString() == request.Model.DayName)
                //            {
                //                var obj = await _context.VenueBlockedSetups.SingleOrDefaultAsync(q => q.BlockedDate == request.Model.BlockedDate, cancellation);
                //                if (obj == null)
                //                {
                //                    obj = new VenueBlockedSetup()
                //                    {
                //                        BlockedDate = date,
                //                        IsActive = true,
                //                        MonthName = date.ToString("MMMM"),
                //                        DayName = date.ToString("dddd"),
                //                        YearName = date.ToString("yyyy")
                //                    };
                //                    _context.VenueBlockedSetups.Add(obj);
                //                }
                //            }
                //        }

                //    }
                //    await _context.SaveChangesAsync(cancellation);
                //}
                //else if( request.Model.IsThisMonth)
                //{
                //    DateTime currentDate = DateTime.Now;
                //    DateTime startDate = new DateTime(currentDate.Year, currentDate.Month, 1);
                //    DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                //    for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                //    {
                //        if (request.Model.DayName != null)
                //        {
                //            if (date.DayOfWeek.ToString() ==request.Model.DayName)
                //            {
                //                var obj = await _context.VenueBlockedSetups.SingleOrDefaultAsync(q => q.BlockedDate == request.Model.BlockedDate, cancellation);
                //                if (obj == null)
                //                {
                //                    obj = new VenueBlockedSetup()
                //                    {
                //                        BlockedDate = date,
                //                        IsActive = true,
                //                        MonthName = date.ToString("MMMM"),
                //                        DayName = date.ToString("dddd"),
                //                        YearName = date.ToString("yyyy")
                //                    };
                //                    _context.VenueBlockedSetups.Add(obj);                                  
                //                }
                //            }                        }

                //    }
                //    await _context.SaveChangesAsync(cancellation);
                //}
                //else
                //{

                List<VenueBlockedSetup> venueBlockedSetupList = new List<VenueBlockedSetup>();


                var objList = await _context.VenueBlockedSetups.Where(q => q.BlockedDate == request.Models.FirstOrDefault().BlockedDate).ToListAsync( cancellation);
                _context.VenueBlockedSetups.RemoveRange(objList);

                foreach (var d in request.Models)
                {
                    if (d.IsChecked)
                    {
                        if(d.VenueAvailableDetails.Count > 0)
                        {
                            foreach(var d1 in d.VenueAvailableDetails)
                            {
                                if (d1.IsChecked)
                                {
                                    var obj = new VenueBlockedSetup()
                                    {
                                        BlockedDate = d.BlockedDate.Value.Date,
                                        IsActive = true,
                                        MonthName = d.BlockedDate == null ? DateTime.Now.ToString("MMMM") : d.BlockedDate.Value.ToString("MMMM"),
                                        DayName = d.BlockedDate == null ? DateTime.Now.ToString("dddd") : d.BlockedDate.Value.ToString("dddd"),
                                        YearName = d.BlockedDate == null ? DateTime.Now.ToString("yyyy") : d.BlockedDate.Value.ToString("yyyy"),
                                        VenueId = d.VenueId,
                                        AvailabilityId = d1.AvailableId,
                                        VenueTitle = d.VenueTitle ?? ""
                                    };
                                    venueBlockedSetupList.Add(obj);
                                }
                                
                            }

                        }
                        else
                        {
                            var obj = new VenueBlockedSetup()
                            {
                                BlockedDate = d.BlockedDate.Value.Date,
                                IsActive = true,
                                MonthName = d.BlockedDate == null ? DateTime.Now.ToString("MMMM") : d.BlockedDate.Value.ToString("MMMM"),
                                DayName = d.BlockedDate == null ? DateTime.Now.ToString("dddd") : d.BlockedDate.Value.ToString("dddd"),
                                YearName = d.BlockedDate == null ? DateTime.Now.ToString("yyyy") : d.BlockedDate.Value.ToString("yyyy"),
                                VenueId = d.VenueId,
                                AvailabilityId = 0,
                                VenueTitle = d.VenueTitle ?? ""
                            };
                        }
                       
                    }
                   
                }
                _context.VenueBlockedSetups.AddRange(venueBlockedSetupList);
                await _context.SaveChangesAsync(cancellation);
                result.HasError = false;
                result.Messages.Add("Venue Blocked");
                // }


                return result;
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }



        }
    }
}
