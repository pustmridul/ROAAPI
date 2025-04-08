using AutoMapper;
using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.MemServices.Models;
using MemApp.Application.Mem.Subscription.Model;
using MemApp.Domain.Entities.ser;
using MemApp.Domain.Entities.subscription;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.MemServices.Queries
{
    public class GetTicketByServiceTypeIdQuery : IRequest<ServiceTicketListVm>
    {
        public int ServiceId { get; set; }
        public int ServiceTypeId { get; set; } 
    }

    public class GetTicketByServiceTypeIdQueryHandler : IRequestHandler<GetTicketByServiceTypeIdQuery, ServiceTicketListVm>
    {
        private readonly IMemDbContext _memdbcontext;
        private readonly IMapper _mapper;

        public GetTicketByServiceTypeIdQueryHandler(IMemDbContext memDbContext, IMapper mapper)
        {
            _memdbcontext = memDbContext;
            _mapper = mapper;
        }

        public async Task<ServiceTicketListVm> Handle(GetTicketByServiceTypeIdQuery request, CancellationToken cancellationToken)
        {
            var result = new ServiceTicketListVm();
            List<ServiceTicket> data = new List<ServiceTicket>();
  
            data = await _memdbcontext.ServiceTickets
              .Include(i=>i.MemServices).ThenInclude(i=>i.ServiceTypes)
              .Where(q=>q.IsActive && q.MemServiceId== request.ServiceId  && q.MemServiceTypeId == request.ServiceTypeId) 
              .ToListAsync(cancellationToken);
               
            result.DataCount = await _memdbcontext.ServiceTickets.CountAsync(cancellationToken);
          

            if (data.Count == 0)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found!");
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
                    Title = s.Title,
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
                    VatChargePercent=s.VatChargePercent
                }).ToList();             
            }
            return result;
        }
    }
}
