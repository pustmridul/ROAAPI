using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Colleges.Models;
using MemApp.Application.Mem.Members.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.MemberTypes.Queries
{
    public class GetAllMemberTypeQuery : IRequest<ListResult<MemberTypeDto>>
    {
        public int Id { get; set; }
    }

    public class GetAllMemberTypeQueryHandler : IRequestHandler<GetAllMemberTypeQuery, ListResult<MemberTypeDto>>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        public GetAllMemberTypeQueryHandler(IMemDbContext context, IPermissionHandler permissionHandler)
        {
            _context = context;
            _permissionHandler = permissionHandler;
        }

        public async Task<ListResult<MemberTypeDto>> Handle(GetAllMemberTypeQuery request, CancellationToken cancellationToken)
        {
            var result = new ListResult<MemberTypeDto>();// MemberTypeListVm();
           
            var data = await _context.MemberTypes
                .Include(i=>i.CategoryPatterns)
                .Where(q => q.IsActive)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            if (data.Count == 0)
            {
                result.HasError = true;
                result.Messages?.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;
                result.Count = data.Count;
                result.Data = data.Select(s => new MemberTypeDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    CategoryName= s.CategoryPatterns.Title,
                    CategoryPatternId= s.CategoryPatternId,
                    IsSubscribed = s.IsSubscribed,
                    OldId= s.OldId
                }).ToList();
            }

            return result;
        }
    }
}
