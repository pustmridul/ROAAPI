using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Models;
using Microsoft.EntityFrameworkCore;


namespace MemApp.Application.Mem.Sales.Command
{
    public class DeleteSaleTicketCommand : IRequest<Result>
    {
        public int Id { get; set; }
    }


    public class DeleteSaleTicketCommandHandler : IRequestHandler<DeleteSaleTicketCommand, Result>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        private readonly ICurrentUserService _currentUserService;
        public DeleteSaleTicketCommandHandler(IMemDbContext context,  ICurrentUserService currentUserService, IPermissionHandler permissionHandler)
        {
            _context = context;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
        }

        public async Task<Result> Handle(DeleteSaleTicketCommand request, CancellationToken cancellationToken)
        {
            var result = new Result();
            if(!await _permissionHandler.HasRolePermissionAsync(104))
            {
                result.HasError = true;
                result.Messages.Add("Permission Denied For Delete Sale");
                return result;
            }
            var data = await _context.SaleMasters.SingleOrDefaultAsync(q=>q.Id==request.Id && q.IsActive, cancellationToken);

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
