using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.AddOnsItems.Command
{
    public class DeleteAddOnsItemCommand : IRequest<Result>
    {
        public int Id { get; set; }
    }


    public class DeleteAddOnsItemCommandHandler : IRequestHandler<DeleteAddOnsItemCommand, Result>
    {
        private readonly IMemDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionHandler _permissionHandler;
        public DeleteAddOnsItemCommandHandler(IMemDbContext context, ICurrentUserService currentUserService, IPermissionHandler permissionHandler)
        {
            _context = context;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
        }

        public async Task<Result> Handle(DeleteAddOnsItemCommand request, CancellationToken cancellationToken)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            var result = new Result();
            if(!await _permissionHandler.HasRolePermissionAsync(3904))
            {
                result.HasError = true;
                result.Messages.Add("Delete AddOnsItem Permission Denied");
                return result;
            }
            try
            {
                var data = await _context.AddOnsItems.SingleOrDefaultAsync(q => q.Id == request.Id && q.IsActive, cancellationToken);

                if (data == null)
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
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Messages.Add("Error"+ ex.Message.ToString());
            }           
            return result;
        }
    }
}
