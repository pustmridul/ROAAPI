using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Members.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.MemberStatuss.Queries
{
    public class GetAllMemberActiveStatusQuery : IRequest<ListResult<MemberActiveStatusDto>>
    {
        public int Id { get; set; }
    }


    public class GetAllMemberActiveStatusQueryHandler : IRequestHandler<GetAllMemberActiveStatusQuery, ListResult<MemberActiveStatusDto>>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        public GetAllMemberActiveStatusQueryHandler(IMemDbContext context, IPermissionHandler permissionHandler)
        {
            _context = context;
            _permissionHandler = permissionHandler;
        }

        public async Task<ListResult<MemberActiveStatusDto>> Handle(GetAllMemberActiveStatusQuery request, CancellationToken cancellationToken)
        {
            var result = new ListResult<MemberActiveStatusDto>();
            if(!await _permissionHandler.HasRolePermissionAsync(1703))
            {
                result.HasError = true;
                result.Messages?.Add("Member Active Status Permission Denied");
                return result;
            }
            var data = await _context.MemberActiveStatuses.Where(q=>q.IsActive)
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
                result.Data =data.Select(s=> new MemberActiveStatusDto
                {
                    Id = s.Id,
                    Name = s.Name
                }).ToList();
            }

            return result;
        }
    }
}
