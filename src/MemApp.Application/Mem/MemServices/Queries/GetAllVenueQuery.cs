using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.MemServices.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.MemServices.Queries
{
    public class GetAllVenueQuery : IRequest<VenueListVm>
    {
        public DateTime TodayDate { get; set; } = DateTime.Now;
    }
    public class GetAllVenueQueryHandler : IRequestHandler<GetAllVenueQuery, VenueListVm>
    {
        private readonly IMemDbContext _context;
        public GetAllVenueQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<VenueListVm> Handle(GetAllVenueQuery request, CancellationToken cancellationToken)
        {
            var result = new VenueListVm();
            try
            {
                var data = await _context.ServiceTickets
                    .Include(i=>i.MemServices)
                    .Include(q=>q.Availability).ThenInclude(i=>i.AvailabilityDetails.Where(c=>c.IsActive))
                    .Include(i=>i.ServiceTicketDetails).ThenInclude(i=>i.ServiceTicketType)
                    .Where(q => q.IsActive && q.MemServiceTypeId==1)
                    .ToPaginatedListAsync(1,100000, cancellationToken);

                if (data.TotalCount == 0)
                {
                    result.HasError = true;
                    result.Messages?.Add("Data Not Found");
                }
                else
                {
                    var venueBlockedData = await _context.VenueBlockedSetups.Where(q => q.BlockedDate == request.TodayDate.Date).ToListAsync(cancellationToken);

                    var bookingSatus= await _context.VenueBookingDetails
                        .Where(q=> q.IsActive && q.BookingDate.Date==request.TodayDate.Date && q.IsCancelled==false).ToListAsync(cancellationToken);


                    result.HasError = false;
                    result.DataCount = data.TotalCount;
                    result.DataList = data.Data.Select(s => new VenueReq
                    {
                        Id = s.Id,
                        Title = s.Title ?? "",
                        ServiceName= s.MemServices.Title,
                        Location=s.Location,
                        Description=s.Description,
                        ImgFileUrl = s.ImgFileUrl,
                        VatPercent=s.VatChargePercent,
                        ServicePercent=s.ServiceChargePercent,

                        VenueAvailabilities=s.Availability !=null ? s.Availability.AvailabilityDetails.Select( sa=> new VenueAvailability
                        {
                            Id=sa.Id,
                            Title=sa.Title,
                            StartTime=DateTime.Parse(sa.StartTime).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            EndTime= DateTime.Parse(sa.EndTime).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            Status= venueBlockedData.Where(q=>q.VenueId==s.MemServiceId && (sa.Id>0? q.AvailabilityId==sa.Id : true)).ToList().Count>0? "Booked" : bookingSatus.FirstOrDefault(q=> q.AvailabilityId ==sa.Id) !=null?"Booked": "Available"
                        }).ToList() : new List<VenueAvailability>(),

                        BookingCriterias= s.ServiceTicketDetails.Select(st=>new BookingCriteria 
                        { 
                            Id=st.ServiceTicketTypeId??0,
                            Price=st.UnitPrice,
                            Title=st.ServiceTicketType != null? st.ServiceTicketType.Title : ""
                        })
                        .ToList(),
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Messages?.Add(ex.ToString());
            }
            
            return result;
        }
    }
}
