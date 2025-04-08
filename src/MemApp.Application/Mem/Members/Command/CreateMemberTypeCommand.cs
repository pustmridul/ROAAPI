using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Colleges.Models;
using MemApp.Application.Mem.Members.Models;
using MemApp.Domain.Entities.mem;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Members.Command
{
    public class CreateMemberTypeCommand : IRequest<Result<MemberTypeDto>>
    {
       // public MemberTypeReq Model { get; set; } = new MemberTypeReq();
        public string Name { get; set; } = string.Empty;
        public int Id { get; set; }
        public int CategoryPatternId { get; set; }
        public bool IsSubscribed { get; set; }
        public string? OldId { get; set; }
    }

    public class MemberTypeMemberTypeCommandHandler : IRequestHandler<CreateMemberTypeCommand, Result<MemberTypeDto>>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionHandler _permissionHandler;
        public MemberTypeMemberTypeCommandHandler(IMemDbContext context, IMediator mediator, ICurrentUserService currentUserService, IPermissionHandler permissionHandler)
        {
            _context = context;
            _mediator = mediator;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
        }
        public async Task<Result<MemberTypeDto>> Handle(CreateMemberTypeCommand request, CancellationToken cancellation)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            var result = new Result<MemberTypeDto>();
            var obj = await _context.MemberTypes
                .SingleOrDefaultAsync(q => q.Id == request.Id);
            if (obj == null)
            {
                if(!await _permissionHandler.HasRolePermissionAsync(1301))
                {
                    result.HasError = true;
                    result.Messages.Add("Permission Denied MemberType created !");
                    return result;
                }

                obj = new MemberType();
                obj.IsActive = true;
                _context.MemberTypes.Add(obj);
                result.HasError = false;
                result.Messages.Add("New MemberType created");
            }
            else
            {
                if (!await _permissionHandler.HasRolePermissionAsync(1302))
                {
                    result.HasError = true;
                    result.Messages.Add("Permission Denied MemberType Update !");
                    return result;
                }
                result.HasError = false;
                result.Messages.Add("MemberType Updated");
            }

            obj.Name = request.Name;
            obj.CategoryPatternId = request.CategoryPatternId;
            obj.IsSubscribed = request.IsSubscribed;
            obj.OldId= request.OldId; 

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