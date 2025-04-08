using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Interfaces;
using MemApp.Application.Mem.ServiceSlottSettings.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.ServiceSlottSettings.Queries
{
    
    public class GetAllServiceSlotSettingsQuery : IRequest<ServiceSlotSettingsListVm>
    {
    }


    public class GetAllServiceSlotSettingsQueryHandler : IRequestHandler<GetAllServiceSlotSettingsQuery, ServiceSlotSettingsListVm>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        public GetAllServiceSlotSettingsQueryHandler(IMemDbContext context, IPermissionHandler permissionHandler)
        {
            _context = context;
            _permissionHandler = permissionHandler;
        }

        public async Task<ServiceSlotSettingsListVm> Handle(GetAllServiceSlotSettingsQuery request, CancellationToken cancellationToken)
        {
            var result = new ServiceSlotSettingsListVm();
            
            try
            {
                var data = await _context.SlotMasters
                    .ToListAsync(cancellationToken);
                    

                if (data.Count() == 0)
                {
                    result.HasError = true;
                    result.Messages?.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
               
                    result.Data = data.Select(s => new ServiceSlotSettingsReq
                    {
                        Id = s.Id,
                        ServiceId = s.ServiceId,
                        ServiceText = s.ServiceText, 

                    }).ToList();
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
