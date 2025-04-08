using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Dashboards.Models;
using System.Text;
using MemApp.Application.Services;
using Dapper;

namespace MemApp.Application.Mem.Dashboards.Queries
{
    public class GetAllDashboardQuery : IRequest<DashboardListVm>
    {
        public int Id { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }


    public class GetAllDashboardQueryHandler : IRequestHandler<GetAllDashboardQuery, DashboardListVm>
    {
        private readonly IMemDbContext _context;
        private readonly IDapperContext _dapperContext;
        public GetAllDashboardQueryHandler(IMemDbContext context, IDapperContext dapperContext)
        {
            _context = context;
            _dapperContext = dapperContext;
        }

        public async Task<DashboardListVm> Handle(GetAllDashboardQuery request, CancellationToken cancellationToken)
        {
            var result = new DashboardListVm();
            try
            {
                //using (var connection = CreateConnection())
                //{
                //    StringBuilder sb = new StringBuilder();
                //    sb.AppendLine(" SELECT * from Customer c JOIN mem_BloodGroup mb ON c.BloodGroupId= mb.Id");

                //    var companies = await connection.QueryAsync<dynamic>(sb.ToString());

                //}


                var data = await _context.Colleges.Where(q => q.IsActive).ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);

                if (data.TotalCount == 0)
                {
                    result.HasError = true;
                    result.Messages.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
                    result.DataCount = data.TotalCount;
                    result.DataList = data.Data.Select(s => new DashboardRes
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Code= s.Code,
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
