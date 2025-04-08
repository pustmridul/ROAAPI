using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.AreaLayouts.Models;
using MemApp.Domain.Entities.mem;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.AreaLayouts.Command
{
    public class CreateAreaLayoutCommand : IRequest<AreaLayoutVm> 
    {
        public AreaLayoutReq Model { get; set; } = new AreaLayoutReq();
    }

    public class CreateAreaLayoutCommandHandler : IRequestHandler<CreateAreaLayoutCommand, AreaLayoutVm>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionHandler _permissionHandler;
        public CreateAreaLayoutCommandHandler(IMemDbContext context, IMediator mediator, ICurrentUserService currentUserService, IPermissionHandler permissionHandler)
        {
            _context = context;
            _mediator = mediator;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
        }
        public async Task<AreaLayoutVm> Handle(CreateAreaLayoutCommand request, CancellationToken cancellation)
        {
            if(_currentUserService?.UserId==null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }

            var result = new AreaLayoutVm();
            try
            {
                var obj = await _context.AreaLayouts
                .Include(x => x.AreaLayoutDetails)
                .SingleOrDefaultAsync(q => q.Id == request.Model.Id);

                if (obj == null)
                {
                    if (!await _permissionHandler.HasRolePermissionAsync(2101))
                    {
                        result.HasError = true;
                        result.Messages.Add("You have no permission to add Area layout");
                        return result;
                    }
                    obj = new AreaLayout();
                    obj.IsActive = true;
                    _context.AreaLayouts.Add(obj);
                    result.HasError = false;
                    result.Messages.Add("New AreaLayout Created");
                }
                else
                {
                    if (!await _permissionHandler.HasRolePermissionAsync(2102))
                    {
                        result.HasError = true;
                        result.Messages.Add("You have no permission to add Area layout");
                        return result;
                    }
                    result.HasError = false;
                    result.Messages.Add("AreaLayout Updated");

                }

                obj.Title = request.Model.Title;
                obj.DisplayName = request.Model.DisplayName;
                obj.ActiveDate = request.Model.ActiveDate;
                obj.Status = request.Model.Status;


                await _context.SaveChangesAsync(cancellation);
                
                var mDeleteObjs = await _context.AreaTableMatrixs
                .Where(q=> request.Model.AreaLayoutDetails.Select(s=>s.AreaLayoutId).Contains(q.AreaLayoutId))
                .ToListAsync();
                _context.AreaTableMatrixs.RemoveRange(mDeleteObjs);

                List<AreaLayoutDetail> deleteObjs = new List<AreaLayoutDetail>();
                foreach (var m in obj.AreaLayoutDetails)
                {
                    var has = request.Model.AreaLayoutDetails.Any(q=>q.AreaLayoutId == m.AreaLayoutId);
                    if (!has)
                    {
                        deleteObjs.Add(m);
                    }
                }

                _context.AreaLayoutDetails.RemoveRange(deleteObjs);
                await _context.SaveChangesAsync(cancellation);
                foreach (var ald in request.Model.AreaLayoutDetails)
                {
                    var oad= obj.AreaLayoutDetails.SingleOrDefault(q=>q.AreaLayoutId==ald.AreaLayoutId);
                    if (oad == null)
                    {
                        oad = new AreaLayoutDetail()
                        {
                            IsActive = true,
                            AreaLayoutId = obj.Id,
                          
                        };
                        _context.AreaLayoutDetails.Add(oad);
                    }

                    oad.TableName = ald.TableName;
                    oad.TableId = ald.TableId;
                    oad.NumberOfChair = ald.NumberOfChair;
                                  
                    List<AreaTableMatrix> matrix = new List<AreaTableMatrix>();
                    for (int i = 1; i <= ald.NumberOfChair; i++)
                    {
                        var atm = new AreaTableMatrix()
                        {
                            AreaLayoutId = obj.Id,
                            TableId = oad.TableId,
                            ChairNo = i,                               
                            IsActive = true
                        };
                        atm.ChairKeyNo = Guid.NewGuid().ToString();
                        matrix.Add(atm);
                    }
                    _context.AreaTableMatrixs.AddRange(matrix);

                }  
                await _context.SaveChangesAsync(cancellation);
        
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Messages.Add("Error " + ex.Message);
            }
            return result;
        }
    }

}
