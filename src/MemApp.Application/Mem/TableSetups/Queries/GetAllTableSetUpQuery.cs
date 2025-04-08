using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.TableSetups.Models;

namespace MemApp.Application.Mem.TableSetups.Queries
{
    public class GetAllTableSetupQuery : IRequest<TableSetupListVm>
    {
        public int Id { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }


    public class GetAllTableSetupQueryHandler : IRequestHandler<GetAllTableSetupQuery, TableSetupListVm>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        public GetAllTableSetupQueryHandler(IMemDbContext context, IPermissionHandler permissionHandler)
        {
            _context = context;
            _permissionHandler = permissionHandler;
        }

        public async Task<TableSetupListVm> Handle(GetAllTableSetupQuery request, CancellationToken cancellationToken)
        {
            var result = new TableSetupListVm();
            if(!await _permissionHandler.HasRolePermissionAsync(2203))
            {
                result.HasError = true;
                result.Messages.Add("Table Setup Viewing Permission Denied");
                return result;
            }
            try
            {    
                var data = await _context.TableSetups.Where(q => q.IsActive)
                    .ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);

                if (data.TotalCount == 0)
                {
                    result.HasError = true;
                    result.Messages.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
                    result.DataCount = data.TotalCount;
                    result.DataList = data.Data.Select(s => new TableSetupReq
                    {
                        Id = s.Id,
                        Title = s.Title,
                        DisplayName = s.DisplayName,
                        IsActive = s.IsActive,
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Messages.Add(ex.ToString());
            }
            
            return result;
        }
    }
}
