using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.MemServices.Models;
using MemApp.Application.Models;
using MemApp.Domain.Entities.ser;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.ServiceTickets.Command
{
    public class ServiceTicketChangeStatusCommand : IRequest<Result>
    {
        public int ServiceTicketId { get; set; } 
    }

    public class ServiceTicketChangeStatusCommandHandler : IRequestHandler<ServiceTicketChangeStatusCommand, Result>
    {
        private readonly IMemDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionHandler _permissionHandler;
        public ServiceTicketChangeStatusCommandHandler(IMemDbContext context,  ICurrentUserService currentUserService,  IPermissionHandler permissionHandler)
        {
            _context = context;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
        }
        public async Task<Result> Handle(ServiceTicketChangeStatusCommand request, CancellationToken cancellation)
        {
            if (_currentUserService.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            var result = new Result();
            try
            {
                if (!await _permissionHandler.HasRolePermissionAsync(2905))
                {
                    result.HasError = true;
                    result.Messages.Add("Permission Denied");
                    return result;
                }
                var obj = await _context.ServiceTickets
                .SingleOrDefaultAsync(q => q.Id == request.ServiceTicketId);
                if (obj == null)
                {
                    result.HasError = true;
                    result.Messages.Add("Ticket not found");
                }
                else
                {
                    obj.Status = !obj.Status;
                    result.HasError = false;
                    result.Messages.Add("Ticket Status Changed");
                }

              
                if (await _context.SaveChangesAsync(cancellation) > 0)
                {
                   
                }
                else
                {
                    result.HasError = true;
                    result.Messages.Add("something wrong");
                }

            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Messages.Add("Exception" + ex.Message);
            }


            return result;
        }
    }

}