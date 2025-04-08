using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.MemServices.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.MemServices.Queries
{
    public class GetServiceAvailibilityDetailByIdQuery : IRequest<ServiceAvailabilityListVm>
    {
        public int ServiceTicketId { get; set; }
    }


    public class GetServiceAvailibilityDetailByIdQueryHandler : IRequestHandler<GetServiceAvailibilityDetailByIdQuery, ServiceAvailabilityListVm>
    {
        private readonly IMemDbContext _context;
        public GetServiceAvailibilityDetailByIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceAvailabilityListVm> Handle(GetServiceAvailibilityDetailByIdQuery request, CancellationToken cancellationToken)
        {
            var result = new ServiceAvailabilityListVm();
            try
            {
                var data = await _context.ServiceAvailabilities.Where(q => q.ServiceTicketId== request.ServiceTicketId).ToListAsync(cancellationToken);

                if (data.Count==0)
                {
                    result.HasError = true;
                    result.Messages?.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
                    result.DataList = data.Select(x => new ServiceAvailabilityRes
                    {
                        Id = x.Id,
                        Afternoon = x.Afternoon,
                        Evening = x.Evening,
                        Morning = x.Evening,
                        AvailabiltyDate = x.AvailabiltyDate
                    }).ToList();
                    result.Messages?.Add("Data Retrieve Successfully");
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
