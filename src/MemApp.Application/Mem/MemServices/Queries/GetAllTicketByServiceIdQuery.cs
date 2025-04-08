using AutoMapper;
using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.MemServices.Models;
using MemApp.Domain.Entities.ser;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.MemServices.Queries
{
    public class GetAllTicketByServiceIdQuery : IRequest<ServiceTicketListVm>
    {
        public int ServiceId { get; set; }
       
    }

    public class GetAllTicketByServiceIdQueryHandler : IRequestHandler<GetAllTicketByServiceIdQuery, ServiceTicketListVm>
    {
        private readonly IMemDbContext _memdbcontext;
        private readonly IMapper _mapper;

        public GetAllTicketByServiceIdQueryHandler(IMemDbContext memDbContext, IMapper mapper)
        {
            _memdbcontext = memDbContext;
            _mapper = mapper;
        }

        public async Task<ServiceTicketListVm> Handle(GetAllTicketByServiceIdQuery request, CancellationToken cancellationToken)
        {
            var result = new ServiceTicketListVm();
            List<ServiceTicket> data = new List<ServiceTicket>();
  
            data = await _memdbcontext.ServiceTickets
                .Include(i=>i.ServiceTicketDetails)
                .Include(i=> i.Availability)
              .Include(i=>i.MemServices).ThenInclude(i=>i.ServiceTypes)
              .Where(q=>q.IsActive && q.MemServiceId== request.ServiceId).Take(1)
              .ToListAsync(cancellationToken);
               
            result.DataCount = await _memdbcontext.ServiceTickets.CountAsync(cancellationToken);
          

            if (data.Count == 0)
            {
                result.HasError = true;
                result.Messages?.Add("Data Not Found!");
            }
            else
            {
                result.HasError = false;
                result.DataList = data.Select(s=> new ServiceTicketReq
                {
                    Id = s.Id,
                    MemServiceText= s.MemServices.Title,
                    MemberServiceTypeText= s.MemServices.ServiceTypes.Title,
                    MemServiceId= s.MemServiceId??0,
                    MemServiceTypeId= s.MemServiceTypeId??0,
                    Title = s.Title ?? s.MemServices.Title,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    PromoCode=s.PromoCode,
                    Location=s.Location,
                    Description=s.Description,
                    HasAvailability=s.HasAvailability,
                    HasTicket=s.HasTicket,
                    ServiceChargeAmount=s.ServiceChargeAmount,
                    ServiceChargePercent=s.ServiceChargePercent,
                    VatChargeAmount=s.VatChargeAmount,
                    VatChargePercent=s.VatChargePercent,                 
                    Status=s.Status,
                    ServiceTicketDetailReqs = s.ServiceTicketDetails.Select(s=> 
                    new ServiceTicketDetailReq 
                    { 
                        Id = s.ServiceTicketTypeId??0,
                        MaxQuantity = s.MaxQuantity,
                        Quantity = s.Quantity,
                        ServiceTicketId= s.ServiceTicketId,
                        ServiceTicketTypeId= s.ServiceTicketTypeId,
                        UnitPrice= s.UnitPrice,
                        TicketType= s.TicketType,
                    }).ToList(),
                }).ToList();             
            }
            return result;
        }
    }
}
