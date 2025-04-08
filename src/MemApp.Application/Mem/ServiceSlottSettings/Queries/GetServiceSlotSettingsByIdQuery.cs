using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Interfaces;
using MemApp.Application.Mem.ServiceSlottSettings.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.ServiceSlottSettings.Queries;

public class GetServiceSlotSettingsByIdQuery : IRequest<ServiceSlotSettingsVm>
{
    public int Id { get; set; }
    public int? ServiceId { get; set; }
}


public class GetServiceSlotSettingsByIdQueryHandler : IRequestHandler<GetServiceSlotSettingsByIdQuery, ServiceSlotSettingsVm>
{
    private readonly IMemDbContext _context;
    private readonly IPermissionHandler _permissionHandler;
    public GetServiceSlotSettingsByIdQueryHandler(IMemDbContext context, IPermissionHandler permissionHandler)
    {
        _context = context;
        _permissionHandler = permissionHandler;
    }

    public async Task<ServiceSlotSettingsVm> Handle(GetServiceSlotSettingsByIdQuery request, CancellationToken cancellationToken)
    {
        var result = new ServiceSlotSettingsVm();
        
        try
        {
            if (request.Id == 0 && request.ServiceId!=null && request.ServiceId>0) {
             
                var  data = await _context.SlotMasters
                 .Include(i => i.ServiceSlots.Where(q => q.IsActive))
                 .SingleOrDefaultAsync(q => q.ServiceId == request.ServiceId, cancellationToken);


                if (data == null)
                {
                    result.HasError = true;
                    result.Messages?.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
                    result.Data.Id = data.Id;
                    result.Succeeded = true;
                    result.Data.ServiceId = data.ServiceId;
                    result.Data.ServiceText = data.ServiceText;

                    result.Data.SlotList = data.ServiceSlots.Select(s => new Slot
                    {
                        Id = s.Id,
                        DayText = s.Day,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime,
                        IsWholeDay = s.IsWholeDay,
                        Qty = 0,
                        IsChecked = false
                    }).ToList();
                }
            }
            else
            {
              var  data = await _context.SlotMasters
                  .Include(i => i.ServiceSlots.Where(q => q.IsActive))
                  .SingleOrDefaultAsync(q => q.Id == request.Id, cancellationToken);


                if (data == null)
                {
                    result.HasError = true;
                    result.Messages?.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
                    result.Data.Id = data.Id;
                    result.Succeeded = true;
                    result.Data.ServiceId = data.ServiceId;
                    result.Data.ServiceText = data.ServiceText;
                    result.Data.SlotList = data.ServiceSlots.Select(s => new Slot
                    {
                        Id = s.Id,
                        DayText = s.Day,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime,
                        IsWholeDay = s.IsWholeDay
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
