using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Services;
using MemApp.Application.Interfaces;
using MemApp.Application.Mem.Booking.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Booking.Queries
{
    public class GetAllVenueBlockedDateQuery : IRequest<VenueBlockedListVm>
    {
        public int PageNo  { get; set; }
        public int PageSize { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set;}

    }


    public class GetAllVenueBlockedDateQueryHandler : IRequestHandler<GetAllVenueBlockedDateQuery, VenueBlockedListVm>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        
        public GetAllVenueBlockedDateQueryHandler(IMemDbContext context,IPermissionHandler permissionHandler)
        {
            _context = context;
            _permissionHandler = permissionHandler;

        }

        public async Task<VenueBlockedListVm> Handle(GetAllVenueBlockedDateQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var result = new VenueBlockedListVm();
                var dataList = await _context.VenueBlockedSetups.Where(q => q.IsActive
                && (request.StartDate != null ? q.BlockedDate.Date >= request.StartDate.Value.Date : true)
                && ( request.EndDate !=null ? q.BlockedDate <= request.EndDate.Value.Date : true)).OrderBy(o=>o.BlockedDate)
                    .ToPaginatedListAsync(request.PageNo, request.PageSize, cancellationToken);

                result.DataList= dataList.Data.Select(s=> new VenueBlockedSetupReq
                {
                    BlockedDate= s.BlockedDate,
                    SelectedDate=s.BlockedDate.ToString("yyyy-MM-dd"),
                    DayName= s.DayName,
                    VenueTitle = s.VenueTitle
                }).ToList();

                result.DataCount= dataList.TotalCount;

                return result;


            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
