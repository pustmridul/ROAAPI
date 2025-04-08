using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Members.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.MemberStatuss.Queries
{
    public class GetMemberActiveStatusByIdQuery : IRequest<Result<MemberActiveStatusDto>>
    {
        public int Id { get; set; }
    }

    public class GetMemberActiveStatusByIdQueryHandler : IRequestHandler<GetMemberActiveStatusByIdQuery, Result<MemberActiveStatusDto>>
    {
        private readonly IMemDbContext _context;
        public GetMemberActiveStatusByIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<Result<MemberActiveStatusDto>> Handle(GetMemberActiveStatusByIdQuery request, CancellationToken cancellationToken)
        {
            var result = new Result<MemberActiveStatusDto>();
            var data = await _context.MemberActiveStatuses.FirstOrDefaultAsync(q => q.Id == request.Id);
            if (data == null)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;
                result.Messages.Add("Data Retrieve Successfully");
                result.Data = new MemberActiveStatusDto { Id = data.Id , Name = data.Name };
            }
            return result;
        }
    }
}
