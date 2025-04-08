using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.MiscItems.Models;
using MemApp.Application.Services;
using MemApp.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.MiscItems.Queries
{
    public class GetAllMiscItemQuery : IRequest<MiscItemListVm>
    {
        public int Id { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class GetAllMiscItemQueryHandler : IRequestHandler<GetAllMiscItemQuery, MiscItemListVm>
    {
        private readonly IMemDbContext _context;
        private readonly IDapperContext _dapperContext;
        private readonly IPermissionHandler _permissionHandler;
        
        public GetAllMiscItemQueryHandler(IMemDbContext context, IDapperContext dapperContext, IPermissionHandler permissionHandler)
        {
            _context = context;
            _dapperContext = dapperContext;
            _permissionHandler = permissionHandler;

        }

        public async Task<MiscItemListVm> Handle(GetAllMiscItemQuery request, CancellationToken cancellationToken)
        {
            var result = new MiscItemListVm();
            try
            {
                var data = await _context.MiscItems
                    .AsNoTracking()
                    .ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);

                if (data.TotalCount == 0)
                {
                    result.HasError = true;
                    result.Messages?.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
                    result.Count = Convert.ToInt32( data.TotalCount);
                    result.Data = data.Data.Select(s => new MiscItemReq
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Price=s.Price,
                        Description=s.Description,
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Messages?.Add(ex.ToString());
            }
            
            return result;
        }
    }
}
