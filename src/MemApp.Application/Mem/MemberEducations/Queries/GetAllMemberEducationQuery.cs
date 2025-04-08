using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;

using System.Text;
using MemApp.Application.Services;
using Dapper;
using MemApp.Application.Interfaces;
using MemApp.Application.Mem.MemberEducations.Models;

namespace MemApp.Application.Mem.MemberEducations.Queries
{
    public class GetAllMemberEducationQuery : IRequest<MemberEducationeListVm>
    {
        public int Id { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }


    public class GetAllMemberEducationQueryHandler : IRequestHandler<GetAllMemberEducationQuery, MemberEducationeListVm>
    {
        private readonly IMemDbContext _context;
        private readonly IDapperContext _dapperContext;
        private readonly IPermissionHandler _permissionHandler;
        
        public GetAllMemberEducationQueryHandler(IMemDbContext context, IDapperContext dapperContext, IPermissionHandler permissionHandler)
        {
            _context = context;
            _dapperContext = dapperContext;
            _permissionHandler = permissionHandler;

        }

        public async Task<MemberEducationeListVm> Handle(GetAllMemberEducationQuery request, CancellationToken cancellationToken)
        {
            var result = new MemberEducationeListVm();
            if (!await _permissionHandler.HasRolePermissionAsync(1203))
            {
                result.HasError = true;
                result.Messages.Add("You have no permission to view");
                return result;
            }

            try
            {
                //using (var connection = CreateConnection())
                //{
                //    StringBuilder sb = new StringBuilder();
                //    sb.AppendLine(" SELECT * from Customer c JOIN mem_BloodGroup mb ON c.BloodGroupId= mb.Id");

                //    var companies = await connection.QueryAsync<dynamic>(sb.ToString());

                //}


                var data = await _context.MemberEducations.ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);

                if (data.TotalCount == 0)
                {
                    result.HasError = true;
                    result.Messages.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
                    result.DataCount = data.TotalCount;
                    result.DataList = data.Data.Select(s => new MemberEducationeRes
                    {
                        Id = s.Id
                        
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
