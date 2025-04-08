using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Members.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.MemberStatuss.Queries
{
    public class GetAllMemberStatusQuery : IRequest<ListResult<MemberStatusDto>>
    {
        public int Id { get; set; }
    }


    public class GetAllMemberStatusQueryHandler : IRequestHandler<GetAllMemberStatusQuery, ListResult<MemberStatusDto>>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        public GetAllMemberStatusQueryHandler(IMemDbContext context, IPermissionHandler permissionHandler)
        {
            _context = context;
            _permissionHandler = permissionHandler;
        }

        public async Task<ListResult<MemberStatusDto>> Handle(GetAllMemberStatusQuery request, CancellationToken cancellationToken)
        {
            var result = new ListResult<MemberStatusDto>();
            if(!await _permissionHandler.HasRolePermissionAsync(1703))
            {
                result.HasError = true;
                result.Messages?.Add("Member Status Viewing Permission Denied");
                return result;
            }
            var data = await _context.MemberStatuses.Where(q => q.IsActive)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            if (data.Count==0)
            {
                result.HasError = true;
                result.Messages?.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;
                result.Count= data.Count;
                result.Data =data.Select(s=> new MemberStatusDto
                {
                    Id = s.Id,
                    Name = s.Name
                }).ToList();
            }

            return result;
        }
    }
}
