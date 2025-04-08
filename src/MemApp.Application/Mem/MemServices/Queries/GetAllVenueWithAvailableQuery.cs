using Dapper;
using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.MemServices.Models;
using MemApp.Application.Mem.Reports.AppDownloadReport.Model;
using MemApp.Application.Services;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace MemApp.Application.Mem.MemServices.Queries
{
    public class GetAllVenueWithAvailableQuery : IRequest<VenueListWithAvailableListVm>
    {
        public DateTime? SelectedDate { get; set; }
    }


    public class GetAllVenueWithAvailableQueryHandler : IRequestHandler<GetAllVenueWithAvailableQuery, VenueListWithAvailableListVm>
    {
        private readonly IMemDbContext _context;
        private readonly IDapperContext _dapperContext;
        public GetAllVenueWithAvailableQueryHandler(IMemDbContext context, IDapperContext dapperContext)
        {
            _context = context;
            _dapperContext = dapperContext;
        }

        public async Task<VenueListWithAvailableListVm> Handle(GetAllVenueWithAvailableQuery request, CancellationToken cancellationToken)
        {
            var result = new VenueListWithAvailableListVm();          
            try
            {
                

                using (var connection = _dapperContext.CreateConnection())
                {


                    try
                    {
                        StringBuilder sb = new StringBuilder();

                        sb.Append("select BlockedDate, VenueId, AvailabilityId, VenueTitle from mem_VenueBlockedSetup where BlockedDate = '"+request.SelectedDate+"' AND IsActive=1");
                        var blockedQuery = await connection
                           .QueryAsync<VenueBlockedDto>(sb.ToString());
                        var blockedData = blockedQuery.ToList();

                        sb = new StringBuilder();

                        sb.Append("select ms.Title AS VenueTitle, ms.Id AS MemserviceId , mad.Title AS AvailabilityTitle, mad.Id AS AvailabilityId from mem_ServiceTicket st " +
                            "JOIN mem_Availability ma ON ma.Id= st.AvailabilityId " +
                            "JOIN mem_AvailabilityDetail mad ON mad.AvailabilityId= ma.Id " +
                            "JOIN mem_MemService ms ON ms.Id= st.MemServiceId " +
                            "WHERE ms.ServiceTypeId=1 " +
                            " AND ms.IsActive=1" +
                            " AND ma.IsActive=1" +
                            " AND mad.IsActive=1" +
                            " AND st.IsActive=1");


                        var dataQuery = await connection
                            .QueryAsync<VenueDto>(sb.ToString());
                        var venueIdList = dataQuery.GroupBy(g=>g.MemserviceId).Select(s=>s.First().MemserviceId).ToList();
                        var data = dataQuery.ToList();


                        

                        foreach( var item in venueIdList)
                        {
                            var venue = data.Where(q => q.MemserviceId == item).ToList();

                            var obj = new VenueListWithAvailable()
                            {
                                BlockedDate= request.SelectedDate,
                                VenueId=item,
                                VenueTitle=venue.First().VenueTitle,
                                IsChecked= blockedData.FirstOrDefault(q=>q.VenueId==item)!=null ?true: false,
                                VenueAvailableDetails= venue.Select(s=>new VenueAvailableDetail
                                {
                                    AvailableId=s.AvailabilityId,
                                    VenueId=item,
                                    AvailableText=s.AvailabilityTitle,
                                    IsChecked= blockedData.FirstOrDefault(q => q.AvailabilityId == s.AvailabilityId) != null ? true : false,
                                } ).ToList(),
                            };

                            result.DataList.Add(obj);
                           
                        }

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                }

                //var venueList =await _context.MemServices.Where(q=>q.ServiceTypeId==7).ToListAsync(cancellationToken);


                //var serviceTickets= await _context.ServiceTickets                  
                //    .Where(q=>q.IsActive && q.MemServiceTypeId==7).ToListAsync(cancellationToken);

                //var availablity= await _context.Availabilities.Include(i=>i.AvailabilityDetails).Where(q=>q.IsActive)
                //    .ToListAsync(cancellationToken);


            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Messages.Add(ex.ToString());
            }
            
            return result;
        }
    }
}
