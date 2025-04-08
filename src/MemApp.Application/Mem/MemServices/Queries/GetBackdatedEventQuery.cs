using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.MemServices.Models;
using MemApp.Application.Mem.Service.Model;
using MemApp.Domain.Entities.ser;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.MemServices.Queries
{
    public class GetBackdatedEventQuery : IRequest<ServiceTicketListVm>
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; } 
    }

    public class GetBackdatedEventQueryHandler : IRequestHandler<GetBackdatedEventQuery, ServiceTicketListVm>
    {
        private readonly IMemDbContext _context;

        public GetBackdatedEventQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceTicketListVm> Handle(GetBackdatedEventQuery request, CancellationToken cancellationToken)
        {
            var result = new ServiceTicketListVm();

            try
            {
                var toDate = request.ToDate ?? DateTime.Now;

                var data = await _context.ServiceTickets
                  .Include(i => i.MemServices)
                      .ThenInclude(i => i.ServiceTypes)
                  .Include(x => x.ServiceTicketDetails)
                  .Where(x =>
                      (request.FromDate == null || x.EventDate.Value.Date > request.FromDate.Value.Date) &&
                      (toDate.Date == null || x.EventDate.Value.Date < toDate.Date) &&
                      x.MemServiceTypeId == 6 
                      && x.IsActive == true
                  ).OrderByDescending(x => x.EventDate)
                  .ToListAsync(cancellationToken);

                if (data == null || !data.Any())
                {
                    result.HasError = true;
                    result.Messages?.Add("Data Not Found");
                    return result;
                }

             
                result.HasError = false;
                result.DataList = data.Select(MapToServiceTicketReq).ToList();
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Messages?.Add(ex.ToString());
            }

            return result;
        }

        private static ServiceTicketReq MapToServiceTicketReq(ServiceTicket data)
        {
            return new ServiceTicketReq
            {
                Id = data.Id,
                Title = data.Title,
                MemServiceId = data.MemServiceId ?? 0,
                MemServiceTitle = data.MemServices?.Title,
                MemServiceTypeId = data.MemServiceTypeId ?? 0,
                MemberServiceTypeText = data.MemServices?.ServiceTypes?.Title,
                StartDate = data.StartDate,
                EndDate = data.EndDate,
                EventDate = data.EventDate,
                HasAreaLayout = data.HasAreaLayout,
                ImgFileUrl = data.ImgFileUrl,
                Description = data.Description,
                PromoCode = data.PromoCode,
                Location = data.Location,
                ServiceChargeAmount = data.ServiceChargeAmount,
                ServiceChargePercent = data.ServiceChargePercent,
                VatChargeAmount = data.VatChargeAmount,
                VatChargePercent = data.VatChargePercent,
                IsActive = data.IsActive,
                AvailabilityId = data.AvailabilityId,
                HasAvailability = data.HasAvailability,
                HasTicket = data.HasTicket,
                TicketLimit = data.TicketLimit,
                ServiceTicketDetailReqs = data.ServiceTicketDetails?.Select(MapToServiceTicketDetailReq).ToList(),
                ServiceTicketAvailabilityReqs = data.ServiceTicketAvailabilities?.Select(MapToServiceTicketAvailabilityReq).ToList(),
                EventTokenReqs = data.EventTokens?.Select(MapToEventTokenReq).ToList(),
                SerTicketAreaLayoutReqs = data.SerTicketAreaLayouts?.Select(MapToSerTicketAreaLayoutReq).ToList(),
                AvailabilityName = data.Availability?.Name ?? "",
                AvailabilityIsLifeTime = data.Availability?.IsLifeTime ?? false,
                AvailabilityDetailList = data.Availability?.AvailabilityDetails?.Select(MapToAvailabilityDetailVm).ToList()
            };
        }

        private static ServiceTicketDetailReq MapToServiceTicketDetailReq(ServiceTicketDetail detail) => new ServiceTicketDetailReq
        {
            Id = detail.Id,
            TicketType = detail.TicketType,
            ServiceTicketTypeId = detail.ServiceTicketTypeId,
            UnitPrice = detail.UnitPrice,
            Quantity = detail.Quantity,
            MaxQuantity = detail.MaxQuantity,
            ServiceTicketId = detail.ServiceTicketId,
            MinQuantity = detail.MinQuantity,
        };

        private static ServiceTicketAvailabilityReq MapToServiceTicketAvailabilityReq(ServiceTicketAvailability availability) => new ServiceTicketAvailabilityReq
        {
            Id = availability.Id,
            DayText = availability.DayText,
            ServiceTicketId = availability.ServiceTicketId,
            StartTime = availability.StartTime,
            EndTime = availability.EndTime,
            Qty = availability.Qty,
            IsWholeDay = availability.IsWholeDay,
            SlotId = availability.SlotId,
            IsChecked = availability.Qty > 0,
        };

        private static EventTokenReq MapToEventTokenReq(EventToken token) => new EventTokenReq
        {
            TokenCode = token.TokenCode,
            Id = token.Id,
            TokenTitle = token.TokenTitle,
            ServiceTicketId = token.ServiceTicketId
        };

        private static SerTicketAreaLayoutReq MapToSerTicketAreaLayoutReq(SerTicketAreaLayout layout) => new SerTicketAreaLayoutReq
        {
            Id = layout.Id,
            AreaLayoutId = layout.AreaLayoutId,
            Title = layout.AreaLayoutTitle ?? "",
            AreaLayoutDetails = layout.SerTicketAreaLayoutMatrices?.Select(MapToSerTicketAreaLayoutMatrixReq).ToList()
        };

        private static SerTicketAreaLayoutMatrixReq MapToSerTicketAreaLayoutMatrixReq(SerTicketAreaLayoutMatrix matrix) => new SerTicketAreaLayoutMatrixReq
        {
            AreaLayoutId = matrix.SerTicketAreaLayoutId,
            NumberOfChair = matrix.NoofChair,
            Id = matrix.Id,
            TableId = matrix.TableId,
            TableName = matrix.TableTitle ?? ""
        };

        private static AvailabilityDetailVm MapToAvailabilityDetailVm(AvailabilityDetail detail) => new AvailabilityDetailVm
        {
            Id = detail.Id,
            StartTime = detail.StartTime,
            EndTime = detail.EndTime,
            IsWholeDay = detail.IsWholeDay
        };
    }
}
