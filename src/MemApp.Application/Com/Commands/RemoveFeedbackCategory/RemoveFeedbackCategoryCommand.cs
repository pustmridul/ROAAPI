using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Interfaces;
using MemApp.Application.Mem.MemberStatuss.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Com.Commands.RemoveFeedbackCategory
{
    public class RemoveFeedbackCategoryCommand : IRequest<Result>
    {
        public int Id { get; set; }
    }
    public class RemoveFeedbackCategoryCommandHandler : IRequestHandler<RemoveFeedbackCategoryCommand, Result>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        private readonly ICurrentUserService _currentUserService;
        public RemoveFeedbackCategoryCommandHandler(IMemDbContext context, IPermissionHandler permissionHandler, ICurrentUserService currentUserService)
        {
            _context = context;
            _permissionHandler = permissionHandler;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(RemoveFeedbackCategoryCommand request, CancellationToken cancellationToken)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            var result = new Result();
            if (!await _permissionHandler.HasRolePermissionAsync(2404))
            {
                result.HasError = true;
                result.Messages.Add("Delete Member Permission Denied");
                return result;
            }

            var data = await _context.FeedbackCategories.FirstOrDefaultAsync(q => q.Id == request.Id, cancellationToken);
           

            


            if (data == null)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                _context.FeedbackCategories.Remove(data);



                await _context.SaveChangesAsync(cancellationToken);
                result.HasError = false;
                result.Messages.Add("Delete Success");
            }

            return result;
        }
    }
}
