using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Colleges.Models;
using MemApp.Application.Mem.MemberEducations.Models;
using MemApp.Domain.Entities.mem;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.MemberEducations.Command
{
    public class CreateMemberEducationCommand : IRequest<MemberEducationeVm>
    {
        public MemberEducationeReq Model { get; set; } = new MemberEducationeReq();
    }

    public class CreateCollegeCommandHandler : IRequestHandler<CreateMemberEducationCommand, MemberEducationeVm>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionHandler _permissionHandler;
        public CreateCollegeCommandHandler(IMemDbContext context, IMediator mediator, ICurrentUserService currentUserService, IPermissionHandler permissionHandler)
        {
            _context = context;
            _mediator = mediator;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
        }
        public async Task<MemberEducationeVm> Handle(CreateMemberEducationCommand request, CancellationToken cancellation)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            var result = new MemberEducationeVm();
            var obj = await _context.MemberEducations
                .SingleOrDefaultAsync(q => q.Id == request.Model.Id);
            if (obj == null)
            {
                //if(!await _permissionHandler.HasRolePermissionAsync(1201))
                //{
                //    result.HasError = true;
                //    result.Messages.Add("New College Creation Permission Denied");
                //    return result;
                //}
                obj = new MemberEducation();
                
                _context.MemberEducations.Add(obj);
                result.HasError = false;
                result.Messages.Add("New Member Education Created");
            }
            else
            {
                //if (!await _permissionHandler.HasRolePermissionAsync(1202))
                //{
                //    result.HasError = true;
                //    result.Messages.Add("MemberEducation Permission Denied");
                //    return result;
                //}
                result.HasError = false;
                result.Messages.Add("Member Education Updated");
            }

            //obj.Name = request.Model.Name;
            //obj.Code = request.Model.Code;
            if (await _context.SaveChangesAsync(cancellation) > 0)
            {
                //result.Data.Name = obj.Name;
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
