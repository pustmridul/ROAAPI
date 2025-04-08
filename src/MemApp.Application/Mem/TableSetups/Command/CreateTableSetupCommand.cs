using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.AreaLayouts.Models;
using MemApp.Application.Mem.AreaLayouts.Queries;
using MemApp.Application.Mem.TableSetups.Command;
using MemApp.Application.Mem.TableSetups.Models;
using MemApp.Application.Mem.TableSetups.Queries;
using MemApp.Domain.Entities.mem;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.AreaLayouts.Command
{
    public class CreateTableSetupCommand : IRequest<TableSetupVm>
    {
        public TableSetupReq Model { get; set; } = new TableSetupReq();
    }

    public class CreateTableSetupCommandHandler : IRequestHandler<CreateTableSetupCommand, TableSetupVm>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        private readonly IUserLogService _userLogService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionHandler _permissionHandler;
        public CreateTableSetupCommandHandler(IMemDbContext context, IMediator mediator, ICurrentUserService currentUserService, IPermissionHandler permissionHandler, IUserLogService userLogService)
        {
            _context = context;
            _mediator = mediator;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
            _userLogService = userLogService;
        }
        public async Task<TableSetupVm> Handle(CreateTableSetupCommand request, CancellationToken cancellation)
        {
            var result = new TableSetupVm();
            var obj = await _context.TableSetups
                .SingleOrDefaultAsync(q => q.Id == request.Model.Id, cancellation);

            if (obj == null)
            {
                if(!await _permissionHandler.HasRolePermissionAsync(2201))
                {
                    result.HasError = true;
                    result.Messages.Add("Table Setup Create Permission Denied");
                    return result;
                }
                obj = new TableSetup();
                obj.IsActive = true;
                _context.TableSetups.Add(obj);
                result.HasError = false;
                result.Messages.Add("New Table Setup Created");
            }
            else
            {
                if (!await _permissionHandler.HasRolePermissionAsync(2202))
                {
                    result.HasError = true;
                    result.Messages.Add("Table Setup Update Permission Denied");
                    return result;
                }
                result.HasError = false;
                result.Messages.Add("Table Setup Updated");
            }

            obj.Title = request.Model.Title;
            obj.DisplayName = request.Model.DisplayName;
          

            if (await _context.SaveChangesAsync(cancellation) > 0)
            {            
                return await _mediator.Send(new GetTableSetupByIdQuery() { Id = obj.Id });
            }
            else
            {
                result.HasError = true;
                result.Messages.Add("something wrong");
                return result;
            }
            
        }
    }

}
