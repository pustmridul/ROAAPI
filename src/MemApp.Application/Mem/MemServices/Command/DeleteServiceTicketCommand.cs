using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.MemServices.Command
{
    public class DeleteServiceTicketCommand : IRequest<Result>
    {
        public int Id { get; set; }
    }


    public class DeleteServiceTicketCommandHandler : IRequestHandler<DeleteServiceTicketCommand, Result>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        private readonly ICurrentUserService _currentUserService;
        public DeleteServiceTicketCommandHandler(IMemDbContext context, IPermissionHandler permissionHandler, ICurrentUserService currentUserService)
        {
            _context = context;
            _permissionHandler = permissionHandler;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(DeleteServiceTicketCommand request, CancellationToken cancellationToken)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            var result = new Result();
            if(!await _permissionHandler.HasRolePermissionAsync(2904))
            {
                result.HasError = true;
                result.Messages.Add("Permission Denied Service Ticket");
                return result;
            }
            var data = await _context.ServiceTickets.FirstOrDefaultAsync(q=>q.Id==request.Id && q.IsActive, cancellationToken);

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
