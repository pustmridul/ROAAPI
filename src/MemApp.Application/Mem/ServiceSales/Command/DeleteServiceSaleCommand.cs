using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.ServiceSales.Command
{
    public class DeleteServiceSaleCommand : IRequest<Result>
    {
        public int Id { get; set; }
    }


    public class DeleteServiceSaleCommandHandler : IRequestHandler<DeleteServiceSaleCommand, Result>
    {
        private readonly IMemDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionHandler _permissionHandler;
        public DeleteServiceSaleCommandHandler(IMemDbContext context, ICurrentUserService currentUserService, IPermissionHandler permissionHandler)
        {
            _context = context;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
        }

        public async Task<Result> Handle(DeleteServiceSaleCommand request, CancellationToken cancellationToken)
        {
            var result = new Result();
            if(!await _permissionHandler.HasRolePermissionAsync(3704))
            {
                result.HasError = true;
                result.Messages.Add("Delete ServiceSale Permission Denied");
                return result;
            }
            try
            {
                var data = await _context.ServiceSales
                    .Include(q=>q.ServiceSaleDetails)
                    .SingleOrDefaultAsync(q => q.Id == request.Id && q.IsActive, cancellationToken);

                if (data == null)
                {
                    result.HasError = true;
                    result.Messages.Add("Data Not Found");
                }
                else
                {
                    data.IsActive = false;
                    foreach (var item in data.ServiceSaleDetails)
                    {
                        item.IsActive = false;
                    }
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
