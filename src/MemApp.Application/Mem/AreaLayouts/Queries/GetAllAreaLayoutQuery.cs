using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.AreaLayouts.Models;
using System.Text;
using MemApp.Application.Services;
using Dapper;
using MemApp.Application.Interfaces;

namespace MemApp.Application.Mem.AreaLayouts.Queries
{
    public class GetAllAreaLayoutQuery : IRequest<AreaLayoutListVm>
    {
        public int Id { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }


    public class GetAllAreaLayoutQueryHandler : IRequestHandler<GetAllAreaLayoutQuery, AreaLayoutListVm>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        public GetAllAreaLayoutQueryHandler(IMemDbContext context, IPermissionHandler permissionHandler)
        {
            _context = context;
            _permissionHandler = permissionHandler;
        }

        public async Task<AreaLayoutListVm> Handle(GetAllAreaLayoutQuery request, CancellationToken cancellationToken)
        {
            var result = new AreaLayoutListVm();
            if(!await _permissionHandler.HasRolePermissionAsync(2103))
            {
                result.HasError = true;
                result.Messages.Add("Area Layout Showing Permission Denied");
                return result;
            }


            try
            {    
                var data = await _context.AreaLayouts.Where(q => q.IsActive)
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
                    result.DataList = data.Data.Select(s => new AreaLayoutReq
                    {
                        Id = s.Id,
                        Title = s.Title,
                        DisplayName = s.DisplayName,
                        Status = s.Status,
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
