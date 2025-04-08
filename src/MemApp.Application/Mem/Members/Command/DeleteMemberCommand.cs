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

namespace MemApp.Application.Mem.MemberStatuss.Command
{
    public class DeleteMemberCommand : IRequest<Result>
    {
        public int Id { get; set; }
    }


    public class DeleteMemberCommandHandler : IRequestHandler<DeleteMemberCommand, Result>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        private readonly ICurrentUserService _currentUserService;
        public DeleteMemberCommandHandler(IMemDbContext context, IPermissionHandler permissionHandler, ICurrentUserService currentUserService)
        {
            _context = context;
            _permissionHandler = permissionHandler;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(DeleteMemberCommand request, CancellationToken cancellationToken)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            var result = new Result();
            if(!await _permissionHandler.HasRolePermissionAsync(2404))
            {
                result.HasError = true;
                result.Messages.Add("Delete Member Permission Denied");
                return result;
            }

            var data = await _context.RegisterMembers.FirstOrDefaultAsync(q=>q.Id==request.Id && q.IsActive, cancellationToken);
            var spouseData= await _context.RegisterMembers.FirstOrDefaultAsync(q => q.MemberId == request.Id && q.IsActive, cancellationToken);
          
            var childData = await _context.MemberChildrens.Where(q => q.RegisterMemberId == request.Id).ToListAsync(cancellationToken);

            var feeData = await _context.MemberFeesMaps.Where(q => q.RegisterMemberId == request.Id).ToListAsync(cancellationToken);


          

            if (data==null)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                data.IsActive = false;
                if(spouseData!=null)
                spouseData.IsActive = false;

                if (childData.Count > 0)
                {
                    foreach(var child in childData)
                    {
                        child.IsActive = false;
                    }
                }
                if (feeData.Count > 0)
                {
                    foreach (var fee in feeData)
                    {
                        fee.IsActive = false;
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);
                result.HasError = false;
                result.Messages.Add("Delete Success");
            }

            return result;
        }
    }
}
