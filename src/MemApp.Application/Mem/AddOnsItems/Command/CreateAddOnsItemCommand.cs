using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.AddOnsItems.Models;
using MemApp.Domain.Entities.mem;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.AddOnsItems.Command
{
    public class CreateAddOnsItemCommand : IRequest<Result>
    {
        public AddOnsItemReq Model { get; set; } = new AddOnsItemReq();
    }

    public class CreateAddOnsItemCommandHandler : IRequestHandler<CreateAddOnsItemCommand, Result>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        private readonly IUserLogService _userLogService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionHandler _permissionHandler;
        public CreateAddOnsItemCommandHandler(IMemDbContext context, IMediator mediator, IUserLogService userLogService, ICurrentUserService currentUserService, IPermissionHandler permissionHandler)
        {
            _context = context;
            _mediator = mediator;
            _userLogService = userLogService;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
        }
        public async Task<Result> Handle(CreateAddOnsItemCommand request, CancellationToken cancellation)
        {
            try
            {
                if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
                {
                    throw new UnauthorizedAccessException();
                }
                var result = new Result();
                var obj = await _context.AddOnsItems
                    .SingleOrDefaultAsync(q => q.Id == request.Model.Id);
                if (obj == null)
                {
                    if (!await _permissionHandler.HasRolePermissionAsync(3901))
                    {
                        result.HasError = true;
                        result.Messages.Add("New AddOnsItem Creation Permission Denied");
                        return result;
                    }
                    obj = new AddOnsItem();
                    obj.IsActive = true;
                    _context.AddOnsItems.Add(obj);
                    result.HasError = false;
                    result.Messages.Add("New AddOnsItem Created");


                }
                else
                {
                    if (!await _permissionHandler.HasRolePermissionAsync(1202))
                    {
                        result.HasError = true;
                        result.Messages.Add("AddOnsItem Update Permission Denied");
                        return result;
                    }
                    result.HasError = false;
                    result.Messages.Add("AddOnsItem Updated");
                }

                obj.Title = request.Model.Title;
                obj.Price = request.Model.Price;
                obj.Description = request.Model.Description;
                obj.PriceDate = DateTime.Parse(request.Model.PriceDate);

                if (await _context.SaveChangesAsync(cancellation) > 0)
                {

                    var dataList = await _context.AddOnsPriceHistorys.Where(q => q.AddOnsItemId == obj.Id).ToListAsync(cancellation);


                    var objdetail = new AddOnsPriceHistory
                    {
                        Price = obj.Price,
                        AddOnsItemId = obj.Id,
                        PriceDate = obj.PriceDate,
                        IsActive = true,
                        ActiveStatus = true
                    };
                    _context.AddOnsPriceHistorys.Add(objdetail);
                    dataList.ForEach(q => q.ActiveStatus = false);
                    await _context.SaveChangesAsync(cancellation);


                }

                return result;
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
           
        }
    }

}
