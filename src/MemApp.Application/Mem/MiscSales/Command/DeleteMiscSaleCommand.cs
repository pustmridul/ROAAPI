using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.MiscSales.Command
{
    public class DeleteMiscSaleCommand : IRequest<Result>
    {
        public int Id { get; set; }
    }

    public class DeleteMiscSaleCommandHandler : IRequestHandler<DeleteMiscSaleCommand, Result>
    {
        private readonly IMemDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionHandler _permissionHandler;
        public DeleteMiscSaleCommandHandler(IMemDbContext context, ICurrentUserService currentUserService, IPermissionHandler permissionHandler)
        {
            _context = context;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
        }

        public async Task<Result> Handle(DeleteMiscSaleCommand request, CancellationToken cancellationToken)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            var result = new Result();
            try
            {
                var data = await _context.MiscSales.SingleOrDefaultAsync(q => q.Id == request.Id, cancellationToken);

                if (data == null)
                {
                    result.HasError = true;
                    result.Messages.Add("Data Not Found");
                }
                else
                {
                    _context.MiscSales.Remove(data);
                    await _context.SaveChangesAsync(cancellationToken);
                    result.HasError = false;
                    result.Messages.Add("Delete Success");
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Messages.Add("Error" + ex.Message.ToString());
            }

            return result;
        }
    }
}
