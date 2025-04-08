using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.AddOnsItems.Models;
using System.Text;
using MemApp.Application.Services;
using Dapper;
using MemApp.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.AddOnsItems.Queries
{
    public class GetAllAddOnsItemQuery : IRequest<AddOnsItemListVm>
    {
        public AddOnsSearchReq Model { get; set; } = new AddOnsSearchReq();
    }


    public class GetAllAddOnsItemQueryHandler : IRequestHandler<GetAllAddOnsItemQuery, AddOnsItemListVm>
    {
        private readonly IMemDbContext _context;
        private readonly IDapperContext _dapperContext;
        private readonly IPermissionHandler _permissionHandler;
        
        public GetAllAddOnsItemQueryHandler(IMemDbContext context, IDapperContext dapperContext, IPermissionHandler permissionHandler)
        {
            _context = context;
            _dapperContext = dapperContext;
            _permissionHandler = permissionHandler;
        }

        public async Task<AddOnsItemListVm> Handle(GetAllAddOnsItemQuery request, CancellationToken cancellationToken)
        {
            var result = new AddOnsItemListVm();      

            try
            {             
                var data = await _context.AddOnsItems
                    .Where(q => q.IsActive)
                    .AsNoTracking()
                    .ToPaginatedListAsync(request.Model.PageNo, request.Model.PageSize, cancellationToken);

                if (data.TotalCount == 0)
                {
                    result.HasError = true;
                    result.Messages?.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
                    result.DataCount = data.TotalCount;
                    result.DataList = data.Data.Select(s => new AddOnsItemReq
                    {
                        Id = s.Id,
                        Title = s.Title,
                        Description = s.Description,
                        Price = s.Price,
                        PriceDate = s.PriceDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
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
