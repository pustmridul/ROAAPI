using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Members.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.MemberTypes.Queries
{
    public class GetMemberTypeByICategorydQuery : IRequest<ListResult<MemberTypeDto>>
    {
        public int CategoryId { get; set; }
    }


    public class GetMemberTypeByICategorydQueryHandler : IRequestHandler<GetMemberTypeByICategorydQuery, ListResult<MemberTypeDto>>
    {
        private readonly IMemDbContext _context;
        public GetMemberTypeByICategorydQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        async Task<ListResult<MemberTypeDto>> IRequestHandler<GetMemberTypeByICategorydQuery, ListResult<MemberTypeDto>>.Handle(GetMemberTypeByICategorydQuery request, CancellationToken cancellationToken)
        {
            var result = new ListResult<MemberTypeDto>();// MemberTypeListVm();
            var data = await _context.MemberTypes.Where(q => q.CategoryPatternId == request.CategoryId).ToListAsync();
            if (data == null)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;
                result.Data =data.Select(x=> new MemberTypeDto { Id = x.Id , Name = x.Name}).ToList();
                result.Messages.Add("Data Retrieve Successfully");
            }

            return result;
        }
    }
}
