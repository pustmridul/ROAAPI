using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Colleges.Models;
using System.Text;
using MemApp.Application.Services;
using Dapper;
using MemApp.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Colleges.Queries
{
    public class GetAllCollegeQuery : IRequest< ListResult<CollegeDto>>
    {
        public int Id { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }


    public class GetAllCollegeQueryHandler : IRequestHandler<GetAllCollegeQuery, ListResult<CollegeDto>>
    {
        private readonly IMemDbContext _context;
        private readonly IDapperContext _dapperContext;
        private readonly IPermissionHandler _permissionHandler;
        
        public GetAllCollegeQueryHandler(IMemDbContext context, IDapperContext dapperContext, IPermissionHandler permissionHandler)
        {
            _context = context;
            _dapperContext = dapperContext;
            _permissionHandler = permissionHandler;

        }

        public async Task<ListResult<CollegeDto>> Handle(GetAllCollegeQuery request, CancellationToken cancellationToken)
        {
            var result = new ListResult<CollegeDto>();
            try
            {
                var data = await _context.Colleges.Where(q => q.IsActive)
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
                    result.Count = data.TotalCount;
                    result.Data = data.Data.Select(s => new CollegeDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Code= s.Code,
                        OldId= s.OldId
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
