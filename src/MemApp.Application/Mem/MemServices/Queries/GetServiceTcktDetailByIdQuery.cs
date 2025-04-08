using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.MemServices.Models;
using MemApp.Application.Mem.Service.Model;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.MemServices.Queries
{
    public class GetServiceTcktDetailByIdQuery : IRequest<ServiceTicketVm>
    {
        public int ServiceTicketId { get; set; }
        public DateTime ReservationFrom { get; set; }
        public DateTime ReservationTo { get; set; }
    }


    public class GetServiceTcktDetailByIdQueryHandler : IRequestHandler<GetServiceTcktDetailByIdQuery, ServiceTicketVm>
    {
        private readonly IMemDbContext _context;
        public GetServiceTcktDetailByIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceTicketVm> Handle(GetServiceTcktDetailByIdQuery request, CancellationToken cancellationToken)
        {
            var result = new ServiceTicketVm();
            try
            {
                var data = await _context.ServiceTickets
                    .Include(i => i.Availability)
                    .Include(i => i.Availability).ThenInclude(i => i.AvailabilityDetails)
                    .Include(i => i.MemServices).ThenInclude(i => i.ServiceTypes)
                    .Include(x => x.ServiceTicketDetails)
                    .Include(x => x.EventTokens)
                    .Include(x => x.SerTicketAreaLayouts).ThenInclude(s => s.SerTicketAreaLayoutMatrices)
                    .SingleOrDefaultAsync(q => q.Id == request.ServiceTicketId && q.IsActive, cancellationToken);

                if (data == null)
                {
                    result.HasError = true;
                    result.Messages.Add("Ticket info not found");
                    return result;

                }
                else
                {
                    result.HasError = false;
                    result.Data = new ServiceTicketReq
                    {
                        Id = data.Id,
                        Title = data.Title,
                        MemServiceId = data.MemServiceId ?? 0,
                        MemServiceTitle = data.MemServices.Title,
                        MemServiceTypeId = data.MemServiceTypeId ?? 0,
                        MemberServiceTypeText = data.MemServices.ServiceTypes.Title,
                        StartDate = data.StartDate,
                        EndDate = data.EndDate,
                        HasAreaLayout = data.HasAreaLayout,

                        Description = data.Description,
                        PromoCode = data.PromoCode,
                        Location = data.Location,

                        ServiceChargeAmount = data.ServiceChargeAmount,
                        ServiceChargePercent = data.ServiceChargePercent,
                        VatChargeAmount = data.ServiceChargeAmount,
                        VatChargePercent = data.ServiceChargePercent,
                        IsActive = data.IsActive,

                        HasAvailability = data.HasAvailability,
                        HasTicket = data.HasTicket,

                        EventTokenReqs = data.EventTokens.Select(se => new EventTokenReq
                        {
                            TokenCode = se.TokenCode,
                            Id = se.Id,
                            TokenTitle = se.TokenTitle,
                            ServiceTicketId = se.ServiceTicketId

                        }).ToList(),
                        ServiceTicketDetailReqs = data.ServiceTicketDetails?.Select(s => new ServiceTicketDetailReq
                        {
                            Id = s.Id,
                            TicketType = s.TicketType,
                            ServiceTicketTypeId = s.ServiceTicketTypeId,
                            UnitPrice = s.UnitPrice,
                            Quantity = s.Quantity,
                            MaxQuantity = s.MaxQuantity,
                            ServiceTicketId = s.ServiceTicketId
                        }).ToList(),
                        SerTicketAreaLayoutReqs = data.SerTicketAreaLayouts.Select(s => new SerTicketAreaLayoutReq
                        {
                            Id = s.Id,
                            AreaLayoutId = s.AreaLayoutId,
                            Title = s.AreaLayoutTitle??"",
                            AreaLayoutDetails = s.SerTicketAreaLayoutMatrices.Select(s => new SerTicketAreaLayoutMatrixReq
                            {
                                AreaLayoutId = s.SerTicketAreaLayoutId,
                                NumberOfChair = s.NoofChair,
                                Id = s.Id,
                                TableId = s.TableId,
                                TableName = s.TableTitle??""

                            }).ToList(),
                        }).ToList()
                    };
                    if (data.Availability != null)
                    {
                       

                        var dataList = await _context.ServiceSaleAvailabilities
                            .ToListAsync(cancellationToken);

                    

                        for (DateTime current = request.ReservationFrom; current <= request.ReservationTo; current = current.AddDays(1))
                        {
                            result.Data.AvailabilityName = current.ToString();
                           
                            foreach (var item in data.Availability.AvailabilityDetails)
                            {
                               
                                    AvailabilityDetailVm avm = new AvailabilityDetailVm();

                                    TimeSpan timeFromDateTime1 = DateTime.Parse(item.StartTime).TimeOfDay;
                                    DateTime dateFromDateTime2 = current.Date;
                                    DateTime combinedDateTimeStart = dateFromDateTime2.Add(timeFromDateTime1);

                                    avm.Id = item.Id;
                                    avm.IsChecked = item.IsChecked;
                                    avm.IsWholeDay = item.IsWholeDay;
                                    avm.AvailabilityId = item.AvailabilityId;
                                    avm.StartTime = item.StartTime;
                                    avm.EndTime = item.EndTime;
                                    avm.Title = item.Title;
                                    avm.ReservationFrom = current.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                                    avm.ReservationTo = request.ReservationTo.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                                    var matchingItem = dataList.FirstOrDefault(c => c.ReservationFrom == combinedDateTimeStart);
                                    avm.Status = matchingItem != null ? "Booked" : "Available";
                                    result.Data.AvailabilityDetailList.Add(avm);
                                
                            }
                        }
                        
                    }


                    return result;
                }
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
