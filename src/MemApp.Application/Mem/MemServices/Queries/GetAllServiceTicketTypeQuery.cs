using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using System.Text;
using MemApp.Application.Services;
using Dapper;
using MemApp.Application.Mem.MemServices.Models;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities.services;
using MemApp.Application.Interfaces;

namespace MemApp.Application.Mem.MemServices.Queries
{
    public class GetAllServiceTicketTypeQuery : IRequest<ServiceTicketTypeListVm>
    {
    }


    public class GetAllServiceTicketTypeQueryHandler : IRequestHandler<GetAllServiceTicketTypeQuery, ServiceTicketTypeListVm>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        public GetAllServiceTicketTypeQueryHandler(IMemDbContext context, IPermissionHandler permissionHandler)
        {
            _context = context;
            _permissionHandler = permissionHandler;
        }

        public async Task<ServiceTicketTypeListVm> Handle(GetAllServiceTicketTypeQuery request, CancellationToken cancellationToken)
        {
            var result = new ServiceTicketTypeListVm();
            if(!await _permissionHandler.HasRolePermissionAsync(1903))
            {
                result.HasError = true;
                result.Messages?.Add("Service Ticket Type List Permission Denied");
                return result;
            }
            try
            {
                result.DataList = await _context.ServiceTicketTypes.Where(q => q.IsActive).Select(s=>new ServiceTicketTypeRes
                {
                    Id= s.Id,
                    Title=s.Title,
                    ServiceType= s.ServiceType
                } ).AsNoTracking()
                    .ToListAsync(cancellationToken);
               
                result.DataCount= result.DataList.Count;    
                
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
