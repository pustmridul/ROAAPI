using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Members.Models;
using MemApp.Application.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.MemberTypes.Command
{
    public class DeleteMemberTypeCommand : IRequest<Result>
    {
        public int Id { get; set; }
    }


    public class DeleteMemberTypeCommandHandler : IRequestHandler<DeleteMemberTypeCommand, Result>
    {
        private readonly IMemDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionHandler _permissionHandler;
        public DeleteMemberTypeCommandHandler(IMemDbContext context, ICurrentUserService currentUserService, IPermissionHandler permissionHandler)
        {
            _context = context;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
        }

        public async Task<Result> Handle(DeleteMemberTypeCommand request, CancellationToken cancellationToken)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            var result = new Result();
            if(! await _permissionHandler.HasRolePermissionAsync(1304))
            {
                result.HasError = true;
                result.Messages.Add("Delete MemberType Permission Denied");
                return result;
            }
            var data = await _context.MemberTypes.FirstOrDefaultAsync(q=>q.Id==request.Id && q.IsActive, cancellationToken);

            if (data==null)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                data.IsActive = false;
                await _context.SaveChangesAsync(cancellationToken);
                result.HasError = false;
                result.Messages.Add("Delete Success");
            }

            return result;
        }
    }
}
