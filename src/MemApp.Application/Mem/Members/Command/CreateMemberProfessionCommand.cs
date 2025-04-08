using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
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
    public class CreateMemberProfessionCommand : IRequest<Result<MemberProfessionDto>>
    {
        public string Name { get; set; } = string.Empty;
        public int Id { get; set; }
    }

    public class MemberProfessionMemberProfessionCommandHandler : IRequestHandler<CreateMemberProfessionCommand, Result<MemberProfessionDto>>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionHandler _permissionHandler;
        public MemberProfessionMemberProfessionCommandHandler(IMemDbContext context, IMediator mediator,  ICurrentUserService currentUserService, IPermissionHandler permissionHandler)
        {
            _context = context;
            _mediator = mediator;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
        }
        public async Task<Result<MemberProfessionDto>> Handle(CreateMemberProfessionCommand request, CancellationToken cancellation)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            var result = new Result<MemberProfessionDto>();
            var obj = await _context.MemberProfessions
                .SingleOrDefaultAsync(q => q.Id == request.Id);
            if (obj == null)
            {
                if(!await _permissionHandler.HasRolePermissionAsync(1501))
                {
                    result.HasError = true;
                    result.Messages.Add("Permission Denied when Adding new Member Profession");
                    return result;
                }

                obj = new MemberProfession();
                obj.IsActive = true;
                _context.MemberProfessions.Add(obj);
                result.HasError = false;
                result.Messages.Add("New MemberProfession created");
            }
            else
            {
                if(!await _permissionHandler.HasRolePermissionAsync(1502))
                {
                    result.HasError = true;
                    result.Messages.Add("Permission Denied when UPDATING Member Profession");
                    return result;
                }

                result.HasError = false;
                result.Messages.Add("MemberProfession Updated");
            }

            obj.Name = request.Name;

            if (await _context.SaveChangesAsync(cancellation) > 0)
            {
               // result.Data.Name = obj.Name;
              //  result.Data.Id = obj.Id;
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