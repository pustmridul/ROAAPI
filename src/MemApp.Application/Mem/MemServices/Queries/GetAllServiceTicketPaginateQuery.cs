using AutoMapper;
using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.MemServices.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.MemServices.Queries
{
    public class GetAllServiceTicketPaginateQuery : IRequest<ServiceTicketListVm>
    {
        public TicketSearchReq Model { get; set; }
    }

    public class GetAllServiceTicketPaginateQueryHandler : IRequestHandler<GetAllServiceTicketPaginateQuery, ServiceTicketListVm>
    {
        private readonly IMemDbContext _memdbcontext;
        private readonly IMapper _mapper;

        public GetAllServiceTicketPaginateQueryHandler(IMemDbContext memDbContext, IMapper mapper)
        {
            _memdbcontext = memDbContext;
            _mapper = mapper;
        }

        public async Task<ServiceTicketListVm> Handle(GetAllServiceTicketPaginateQuery request, CancellationToken cancellationToken)
        {
            var result = new ServiceTicketListVm();

            result.DataList = await _memdbcontext.ServiceTickets
                .Include(i => i.MemServices).ThenInclude(i => i.ServiceTypes)
                .Where(q => q.IsActive
                && (request.Model.ServiceId > 0  ? q.MemServiceId == request.Model.ServiceId  : true)
                && (request.Model.ServiceTypeId > 0 ? q.MemServiceTypeId == request.Model.ServiceTypeId : true)
                && (!string.IsNullOrEmpty(request.Model.SearchText) ? (q.Title.Contains(request.Model.SearchText) || (q.PromoCode.Contains(request.Model.SearchText))
                )
                :
                true
                )
                ).OrderByDescending(q => q.Id)
                .Skip((request.Model.PageNo - 1) * request.Model.PageSize)
                .Take(request.Model.PageSize)
                .Select(s => new ServiceTicketReq
                {
                    Id = s.Id,
                    MemServiceText = s.MemServices.Title,
                    MemberServiceTypeText = s.MemServices.ServiceTypes.Title,
                    MemServiceId = s.MemServiceId ?? 0,
                    MemServiceTypeId = s.MemServiceTypeId ?? 0,
                    Title = s.Title,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    PromoCode = s.PromoCode,
                    Location = s.Location,
                    Description = s.Description,
                    HasAvailability = s.HasAvailability,
                    HasTicket = s.HasTicket,
                    ServiceChargeAmount = s.ServiceChargeAmount,
                    ServiceChargePercent = s.ServiceChargePercent,
                    VatChargeAmount = s.VatChargeAmount,
                    VatChargePercent = s.VatChargePercent
                })
                .ToListAsync( cancellationToken);

                   
              result.DataCount = await _memdbcontext.ServiceTickets
                    .Include(i => i.MemServices).ThenInclude(i => i.ServiceTypes)
                    .Where(q => q.IsActive
                    && (request.Model.ServiceId > 0 ? q.MemServiceId == request.Model.ServiceId : true)
                    && (request.Model.ServiceTypeId > 0 ? q.MemServiceTypeId == request.Model.ServiceTypeId : true)
                    && (!string.IsNullOrEmpty(request.Model.SearchText) ? (q.Title.Contains(request.Model.SearchText) || (q.PromoCode.Contains(request.Model.SearchText))
                    )
                    :
                    true
                    )
                ).CountAsync(cancellationToken);
          

            if (result.DataCount == 0)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found!");
            }
                   
            return result;
        }
    }
}
