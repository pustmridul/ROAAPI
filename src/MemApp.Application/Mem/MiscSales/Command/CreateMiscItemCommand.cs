using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.MiscItems.Models;
using MemApp.Domain.Entities.Sale;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.MiscSales.Command
{
    public class CreateMiscItemCommand : IRequest<MiscItemVm>
    {
        public MiscItemReq Model { get; set; } = new MiscItemReq();
    }

    public class CreateMiscItemCommandHandler : IRequestHandler<CreateMiscItemCommand, MiscItemVm>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionHandler _permissionHandler;
        public CreateMiscItemCommandHandler(IMemDbContext context, IMediator mediator,  ICurrentUserService currentUserService, IPermissionHandler permissionHandler)
        {
            _context = context;
            _mediator = mediator;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
        }
        public async Task<MiscItemVm> Handle(CreateMiscItemCommand request, CancellationToken cancellation)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            var result = new MiscItemVm();
            var obj = await _context.MiscItems
                .SingleOrDefaultAsync(q => q.Id == request.Model.Id);
            if (obj == null)
            {
                obj = new MiscItem();
              
                _context.MiscItems.Add(obj);
                result.HasError = false;
                result.Messages.Add("New MiscItem Created");
            }
            obj.Name = request.Model.Name;
            obj.Price = request.Model.Price;
            obj.Description = request.Model.Description;

            if (await _context.SaveChangesAsync(cancellation) > 0)
            {

                result.Data.Name = obj.Name;
                result.Data.Price = obj.Price;
                result.Data.Id = obj.Id;
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
