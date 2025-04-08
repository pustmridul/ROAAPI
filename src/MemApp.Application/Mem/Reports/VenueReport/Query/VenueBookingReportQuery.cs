using Dapper;
using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Members.Models;

using MemApp.Application.Mem.Reports.VenueReport.Model;
using MemApp.Application.Services;
using Microsoft.EntityFrameworkCore;

using System.Text;


namespace MemApp.Application.Mem.Reports.VenueReport.Query
{
    public class VenueBookingReportQuery: IRequest<List<VenueBookingReportVM>>
    {
        public VenueBookingReportCriteria Model { get; set; } = new VenueBookingReportCriteria();
    }

    public class VenueBookingReportQueryHandler : IRequestHandler<VenueBookingReportQuery, List<VenueBookingReportVM>>
    {
        private readonly IDapperContext _context;
        private readonly IMemDbContext _memdbcontext;
        public VenueBookingReportQueryHandler(IDapperContext context, IMemDbContext memdbcontext)
        {
            _context = context;
            _memdbcontext = memdbcontext;

        }
    

        public async Task<List<VenueBookingReportVM>> Handle(VenueBookingReportQuery request, CancellationToken cancellationToken)
        {
            var result = new List<ViewMemberDto>();

            using (var connection = _context.CreateConnection())
            {
                var venueTicket = await _memdbcontext.ServiceTickets.Where(c=>c.MemServiceId== request.Model.VenueId).ToListAsync(cancellationToken);

                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("select * from VW_VenueBooking");

                   
                    

                    var dataQuery = await connection
                        .QueryAsync<VenueBookingReportVM>(sb.ToString());


                    if (!string.IsNullOrEmpty(request.Model.VenueTicketIds))
                    {
                        int[] VenueTicketIds = request.Model.VenueTicketIds.Split(',')
                        .Select(id => int.TryParse(id, out int eventId) ? eventId : -1) // Handle parsing failures with -1 (or other default value)
                        .Where(id => id != -1)
                        .ToArray();
                        dataQuery = dataQuery.Where(c => VenueTicketIds.Contains(c.VenueId));
                    }

                    if (request.Model.FromDate != null)
                    {
                        dataQuery = dataQuery.Where(c => c.BookedDate.Date >= request.Model.FromDate?.Date);
                    }
                    if (request.Model.ToDate != null)
                    {
                        dataQuery = dataQuery.Where(c => c.BookedDate.Date <= request.Model.ToDate?.Date);
                    }

                    var data = dataQuery.ToList();


                    return data;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
        }

    }

}

