using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Members.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.MemberTypes.Queries
{
    public class GetMemberTypeByIdQuery : IRequest<Result<MemberTypeDto>>
    {
        public int Id { get; set; }
    }


    public class GetMemberTypeByIdQueryHandler : IRequestHandler<GetMemberTypeByIdQuery, Result<MemberTypeDto>>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        public GetMemberTypeByIdQueryHandler(IMemDbContext context, IPermissionHandler permissionHandler)
        {
            _context = context;
            _permissionHandler = permissionHandler;
        }

        public async Task<Result<MemberTypeDto>> Handle(GetMemberTypeByIdQuery request, CancellationToken cancellationToken)
        {
            var result = new Result<MemberTypeDto>();
            var data = await _context.MemberTypes
                .Include(i=>i.CategoryPatterns)
                .AsNoTracking()
                .SingleOrDefaultAsync(q=>q.Id==request.Id, cancellationToken);
            if (data==null)
            {
                result.HasError = true;
                result.Messages?.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;

                result.Data = new MemberTypeDto
                {
                    Id = data.Id,
                    Name = data.Name,
                    IsSubscribed = data.IsSubscribed,
                    CategoryName=data.CategoryPatterns.Title,
                    CategoryPatternId=data.CategoryPatternId,
                    OldId= data.OldId,

                };
            }

            return result;
        }
    }
}
