using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Colleges.Models;
using MemApp.Domain.Entities.mem;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Colleges.Command
{
    public class CreateCommCatCommand : IRequest<CommCatVm>
    {
        public CommCatReq Model { get; set; } = new CommCatReq();
    }

    public class CreateCommCatCommandHandler : IRequestHandler<CreateCommCatCommand, CommCatVm>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionHandler _permissionHandler;
        public CreateCommCatCommandHandler(IMemDbContext context, IMediator mediator, ICurrentUserService currentUserService,IPermissionHandler permissionHandler)
        {
            _context = context;
            _mediator = mediator;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
        }
        public async Task<CommCatVm> Handle(CreateCommCatCommand request, CancellationToken cancellation)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }

            var result = new CommCatVm();

            var obj = await _context.CommitteeCategories
                .SingleOrDefaultAsync(q => q.Id == request.Model.Id);
            if (obj == null)
            {

                if (!await _permissionHandler.HasRolePermissionAsync(3401))
                {
                    result.HasError = true;
                    result.Messages.Add("Committee Category Create Permission Denied");
                    return result;
                }

                obj = new CommitteeCategory();
                obj.IsActive = true;
                _context.CommitteeCategories.Add(obj);
                result.HasError = false;
                result.Messages.Add("New Committee Category Created");
            }
            else
            {
                if (!await _permissionHandler.HasRolePermissionAsync(3402))
                {
                    result.HasError = true;
                    result.Messages.Add("Committee Category Update Permission Denied");
                    return result;
                }
                result.HasError = false;
                result.Messages.Add("Committee Category Updated");
            }

            obj.Title = request.Model.Title;
         
            if (await _context.SaveChangesAsync(cancellation) > 0)
            {

                result.Data.Title = obj.Title;
                result.Data.Id = obj.Id;
            }
            else
            {
                result.HasError = true;
                result.Messages.Add("something wrong");
            }
            return result;
        }
    }

}
