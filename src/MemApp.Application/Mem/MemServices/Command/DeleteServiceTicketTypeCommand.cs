using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.MemServices.Command
{
    public class DeleteServiceTicketTypeCommand : IRequest<Result>
    {
        public int Id { get; set; }
    }


    public class DeleteServiceTicketTypeCommandHandler : IRequestHandler<DeleteServiceTicketTypeCommand, Result>
    {
        private readonly IMemDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionHandler _permissionHandler;
        public DeleteServiceTicketTypeCommandHandler(IMemDbContext context, ICurrentUserService currentUserService,  IPermissionHandler permissionHandler)
        {
            _context = context;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
        }

        public async Task<Result> Handle(DeleteServiceTicketTypeCommand request, CancellationToken cancellationToken)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            var result = new Result();
            if(!await _permissionHandler.HasRolePermissionAsync(1904))
            {
                result.HasError = true;
                result.Messages.Add("Delete ServiceTicket Type Permission Denied!!");
                return result;
            }
            var data = await _context.ServiceTicketTypes.FirstOrDefaultAsync(q=>q.Id==request.Id && q.IsActive, cancellationToken);

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
