using AutoMapper;
using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.MemServices.Models;
using MemApp.Domain.Entities.ser;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.MemServices.Queries
{
    public class GetAllServiceTicketByMemServiceQuery : IRequest<ServiceTicketListVm>
    {
        public int MemServiceId { get; set; }
    }

    public class GetAllServiceTicketByMemServiceQueryHandler : IRequestHandler<GetAllServiceTicketByMemServiceQuery, ServiceTicketListVm>
    {
        private readonly IMemDbContext _memdbcontext;
        private readonly IMapper _mapper;

        public GetAllServiceTicketByMemServiceQueryHandler(IMemDbContext memDbContext, IMapper mapper)
        {
            _memdbcontext = memDbContext;
            _mapper = mapper;
        }

        public async Task<ServiceTicketListVm> Handle(GetAllServiceTicketByMemServiceQuery request, CancellationToken cancellationToken)
        {
            var result = new ServiceTicketListVm();
            List<ServiceTicket> data = new List<ServiceTicket>();

            data = await _memdbcontext.ServiceTickets.Where(x => x.MemServiceId == request.MemServiceId)
                .ToListAsync(cancellationToken);
            result.DataCount = data.Count;
            if (data.Count == 0)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found!");
            }
            else
            {
                var mappingData = _mapper.Map<List<ServiceTicketReq>>(data);
                result.HasError = false;
                result.DataList = mappingData;
                result.Messages.Add("Successfully Retrieve Data.");
            }
            return result;
        }
    }
}
