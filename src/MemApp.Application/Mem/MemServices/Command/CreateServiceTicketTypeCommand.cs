using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.MemServices.Models;
using MemApp.Domain.Entities.ser;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.MemServices.Command
{
    public class CreateServiceTicketTypeCommand : IRequest<ServiceTicketTypeVm>
    {
        public ServiceTicketTypeReq Model { get; set; } = new ServiceTicketTypeReq();
    }

    public class CreateServiceTicketTypeCommandHandler : IRequestHandler<CreateServiceTicketTypeCommand, ServiceTicketTypeVm>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionHandler _permissionHandler;
        public CreateServiceTicketTypeCommandHandler(IMemDbContext context, IMediator mediator, ICurrentUserService currentUserService,  IPermissionHandler permissionHandler)
        {
            _context = context;
            _mediator = mediator;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
        }
        public async Task<ServiceTicketTypeVm> Handle(CreateServiceTicketTypeCommand request, CancellationToken cancellation)
        {
            if (_currentUserService.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            var result = new ServiceTicketTypeVm();
            try
            {
                var obj = await _context.ServiceTicketTypes
                .SingleOrDefaultAsync(q => q.Id == request.Model.Id);
                if (obj == null)
                {
                    if(!await _permissionHandler.HasRolePermissionAsync(1901))
                    {
                        result.HasError = true;
                        result.Messages.Add("ServiceTicket Type Creating Permission Denied");
                        return result;
                    }

                    obj = new ServiceTicketType();
                    obj.IsActive = true;
                    _context.ServiceTicketTypes.Add(obj);
                    result.HasError = false;
                    result.Messages.Add("New ServiceTicket Type created");
                }
                else
                {
                    if (!await _permissionHandler.HasRolePermissionAsync(1902))
                    {
                        result.HasError = true;
                        result.Messages.Add("ServiceTicket Type Updating Permission Denied");
                        return result;
                    }
                    result.HasError = false;
                    result.Messages.Add("ServiceTicket Type Updated");
                }

                obj.Title = request.Model.Title;
                obj.ServiceType = request.Model.ServiceType;

                if (await _context.SaveChangesAsync(cancellation) > 0)
                {
                    result.Data.Title = obj.Title;
                    request.Model.ServiceType = obj.ServiceType;
                    result.Data.Id = obj.Id;
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