using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.MemServices.Models;
using MemApp.Application.Mem.Service.Model;
using MemApp.Domain.Entities.ser;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.MemServices.Queries
{
    public class GetServiceTicketByIdQuery : IRequest<ServiceTicketVm>
    {
        public int Id { get; set; }
    }


    public class GetServiceTicketByIdQueryHandler : IRequestHandler<GetServiceTicketByIdQuery, ServiceTicketVm>
    {
        private readonly IMemDbContext _context;
        public GetServiceTicketByIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceTicketVm> Handle(GetServiceTicketByIdQuery request, CancellationToken cancellationToken)
        {
            var result = new ServiceTicketVm();
            try
            {
                var data = await _context.ServiceTickets
                    .Include(i=>i.Availability)
                    .Include(i=>i.Availability).ThenInclude(i=>i.AvailabilityDetails)
                    .Include(i=>i.MemServices).ThenInclude(i=>i.ServiceTypes)
                    .Include(x => x.ServiceTicketDetails)                  
                    .Include(x => x.EventTokens)
                    .Include(x => x.ServiceTicketAvailabilities)
                    .Include(x=>x.SerTicketAreaLayouts).ThenInclude(s=>s.SerTicketAreaLayoutMatrices)
                    .SingleOrDefaultAsync(q => q.Id== request.Id && q.IsActive, cancellationToken);

                if (data == null)
                {
                    result.HasError = true;
                    result.Messages?.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
                    result.Data = new ServiceTicketReq
                    {
                        Id = data.Id,
                        Title = data.Title,
                        MemServiceId = data.MemServiceId ??0,
                        MemServiceTitle = data.MemServices.Title,
                        MemServiceTypeId = data.MemServiceTypeId ?? 0,
                        MemberServiceTypeText= data.MemServices.ServiceTypes.Title,
                        StartDate = data.StartDate,
                        EndDate= data.EndDate,
                        EventDate= data.EventDate,
                        HasAreaLayout=data.HasAreaLayout,
                        ImgFileUrl= data.ImgFileUrl,
                      
                      //  ValidTo= data.ValidTo,
                        Description= data.Description,
                        PromoCode= data.PromoCode,
                        Location= data.Location,
                        ServiceChargeAmount= data.ServiceChargeAmount,
                        ServiceChargePercent= data.ServiceChargePercent,
                        VatChargeAmount= data.VatChargeAmount,
                        VatChargePercent= data.VatChargePercent,
                        IsActive= data.IsActive,
                        AvailabilityId= data.AvailabilityId,                      
                        HasAvailability = data.HasAvailability,
                        HasTicket= data.HasTicket,    
                        TicketLimit= data.TicketLimit,
                        ServiceTicketDetailReqs = data.ServiceTicketDetails?.Select(s => new ServiceTicketDetailReq
                        {
                          Id = s.Id,
                          TicketType = s.TicketType,
                          ServiceTicketTypeId = s.ServiceTicketTypeId,
                          UnitPrice = s.UnitPrice,
                          Quantity = s.Quantity,
                          MaxQuantity = s.MaxQuantity,
                          ServiceTicketId= s.ServiceTicketId,
                          MinQuantity= s.MinQuantity,
                        }).ToList(),
                        ServiceTicketAvailabilityReqs = data.ServiceTicketAvailabilities?.Select(s => new ServiceTicketAvailabilityReq
                        {
                            Id = s.Id,
                            DayText = s.DayText,
                            ServiceTicketId = s.ServiceTicketId,
                            StartTime = s.StartTime,
                            EndTime = s.EndTime,
                            Qty = s.Qty,
                            IsWholeDay = s.IsWholeDay,
                            SlotId = s.SlotId,
                            IsChecked= s.Qty>0? true: false,
                        }).ToList(),
                        EventTokenReqs = data.EventTokens.Select(se=> new EventTokenReq
                        {
                            TokenCode = se.TokenCode,
                            Id = se.Id,
                            TokenTitle = se.TokenTitle,
                            ServiceTicketId= se.ServiceTicketId

                        }).ToList(),
                      
                        SerTicketAreaLayoutReqs  = data.SerTicketAreaLayouts.Select(s=> new SerTicketAreaLayoutReq
                        {
                            Id= s.Id,
                            AreaLayoutId = s.AreaLayoutId,
                            Title = s.AreaLayoutTitle ?? "",
                            AreaLayoutDetails = s.SerTicketAreaLayoutMatrices.Select(s=>new SerTicketAreaLayoutMatrixReq
                            {
                                AreaLayoutId = s.SerTicketAreaLayoutId,
                                NumberOfChair = s.NoofChair,
                                Id = s.Id,
                                TableId = s.TableId,
                                TableName = s.TableTitle??""

                            }).ToList(),
                        }).ToList()
                    };
                    if (data.Availability!=null)
                    {
                        result.Data.AvailabilityName = data.Availability.Name ?? "";
                        result.Data.AvailabilityIsLifeTime = data.Availability.IsLifeTime;
                        result.Data.AvailabilityDetailList = data.Availability.AvailabilityDetails.Select(sa => new AvailabilityDetailVm
                        {
                            Id = sa.Id,
                            StartTime = sa.StartTime,
                            EndTime = sa.EndTime,
                            IsWholeDay = sa.IsWholeDay,

                        }).ToList();
                    }
                    
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
