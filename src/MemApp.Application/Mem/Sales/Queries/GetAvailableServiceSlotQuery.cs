using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Sales.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace MemApp.Application.Mem.Sales.Queries
{
    public class GetAvailableServiceSlotQuery : IRequest<ListResult<AvailableServiceSlotListVm>>
    {
        public DateTime date { get; set; }
        public int? ServiceTicketId { get; set; }
    }

    public class GetAvailableServiceSlotQueryHandler : IRequestHandler<GetAvailableServiceSlotQuery, ListResult<AvailableServiceSlotListVm>>
    {
        private readonly IMemDbContext _context;
        public GetAvailableServiceSlotQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<ListResult<AvailableServiceSlotListVm>> Handle(GetAvailableServiceSlotQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = new ListResult<AvailableServiceSlotListVm>();

                DayOfWeek dayOfWeek = request.date.DayOfWeek;



                List<AvailableServiceSlotListVm> dataList = new List<AvailableServiceSlotListVm>();
                var serviceSale = await _context.ServiceSalesDetails
               .Where(q => q.SeviceTicketAvailablityId > 0 && q.RevDate.Date == request.date.Date)
               .AsNoTracking()
               .ToListAsync(cancellationToken);

                var data = await _context.ServiceTicketAvailabilities
                    .Where(q => q.ServiceTicketId == request.ServiceTicketId && q.DayText == dayOfWeek.ToString()).AsNoTracking()
                    .ToListAsync(cancellationToken);

                foreach (var d in data)
                {
                    if (d.Qty == 0)
                    {
                        continue;
                    }
                    var obj = new AvailableServiceSlotListVm
                    {
                        Id = d.Id,
                        DayText = d.DayText,
                        ServiceTicketId = d.ServiceTicketId,
                        StartTime = d.StartTime ?? "",
                        EndTime = d.EndTime ?? "",
                        IsWholeDay = d.IsWholeDay,
                        Qty = 1,
                        SlotQty = d.Qty,
                        SoldQty = serviceSale.Where(q => q.SeviceTicketAvailablityId == d.Id).Select(s2 => s2.Quantity).Sum() ?? 0,
                    };
                    if (obj.SoldQty >= obj.SlotQty)
                    {
                        obj.IsSoldOut = true;
                    }
                    dataList.Add(obj);
                }
                result.HasError = false;
                result.Data = dataList;
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}
