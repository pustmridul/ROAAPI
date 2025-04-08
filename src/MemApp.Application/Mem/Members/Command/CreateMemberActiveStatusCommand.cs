using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Members.Models;
using MemApp.Domain.Entities.mem;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Members.Command
{
    public class CreateMemberActiveStatusCommand : IRequest<Result<MemberActiveStatusDto>>
    {
        //public MemberActiveStatusReq Model { get; set; } = new MemberActiveStatusReq();
        public string Name { get; set; } = string.Empty;
        public int Id { get; set; }
    }

    public class MemberActiveStatusMemberStatusCommandHandler : IRequestHandler<CreateMemberActiveStatusCommand, Result<MemberActiveStatusDto>>
    {
        private readonly IMemDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionHandler _permissionHandler;
        public MemberActiveStatusMemberStatusCommandHandler(IMemDbContext context,  ICurrentUserService currentUserService,  IPermissionHandler permissionHandler)
        {
            _context = context;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
        }
        public async Task<Result<MemberActiveStatusDto>> Handle(CreateMemberActiveStatusCommand request, CancellationToken cancellation)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            var result = new Result<MemberActiveStatusDto>();
            var obj = await _context.MemberActiveStatuses
                .SingleOrDefaultAsync(q => q.Id == request.Id);
            if (obj == null)
            {
                if(!await _permissionHandler.HasRolePermissionAsync(1701))
                {
                    result.HasError = true;
                    result.Messages.Add("Create Member Active Status Permission Denied");
                    return result;
                }

                obj = new MemberActiveStatus();
                obj.IsActive = true;
                _context.MemberActiveStatuses.Add(obj);
                result.HasError = false;
                result.Messages.Add("New MemberStatus created");
            }
            else
            {
                if (!await _permissionHandler.HasRolePermissionAsync(1702))
                {
                    result.HasError = true;
                    result.Messages.Add("Update Member Active Status Permission Denied");
                    return result;
                }

                result.HasError = false;
                result.Messages.Add("MemberStatus Updated");
            }

            obj.Name = request.Name;

            if (await _context.SaveChangesAsync(cancellation) > 0)
            {

                //result.Data.Name = obj.Name;
                //result.Data.Id = obj.Id;
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