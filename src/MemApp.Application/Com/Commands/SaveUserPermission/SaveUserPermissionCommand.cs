using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Com.Queries.GetUserPermissions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Domain.Entities;

namespace MemApp.Application.Com.Commands.SaveUserPermission
{
    public class SaveUserPermissionCommand : IRequest<PermissionModelListVm>
    {
        public UserPermissionReq model { get; set; } = new UserPermissionReq();

    }

    public class SaveUserPermissionCommandHandler : IRequestHandler<SaveUserPermissionCommand, PermissionModelListVm>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;

        public SaveUserPermissionCommandHandler(
            IMemDbContext context,
            IMediator mediator
            )
        {
            _context = context;
            _mediator = mediator;
           
        }


        public async Task<PermissionModelListVm> Handle(SaveUserPermissionCommand request, CancellationToken cancellation)
        {
            int userId = request.model.UserId;
            foreach (var up in request.model.PermissionList)
            {
                 
                var obj = _context.UserPermissions
                    .FirstOrDefault(q => q.UserId == userId  && q.PermissionNo == up);

                if (obj != null)
                {
                    continue;
                }

                obj = new UserPermission()
                {
                    UserId = userId,
                    PermissionNo = up
                };
                _context.UserPermissions.Add(obj);

            }

            var currentPermissions = _context.UserPermissions
                .Where(up => up.UserId == userId);

            foreach (var cp in currentPermissions)
            {
                if (request.model.PermissionList.Any(up => up == cp.PermissionNo))
                    continue;
                _context.UserPermissions.Remove(cp);

            }
            
            await _context.SaveChangesAsync(cancellation);

            return   await _mediator.Send(new GetPermissionListQuery()
            {
                userId = userId
            });
        }
    }

}