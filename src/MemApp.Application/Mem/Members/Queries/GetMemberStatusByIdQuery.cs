using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Members.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.MemberStatuss.Queries
{
    public class GetMemberStatusByIdQuery : IRequest<Result<MemberStatusDto>>
    {
        public int Id { get; set; }
    }


    public class GetMemberStatusByIdQueryHandler : IRequestHandler<GetMemberStatusByIdQuery, Result<MemberStatusDto>>
    {
        private readonly IMemDbContext _context;
        public GetMemberStatusByIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<Result<MemberStatusDto>> Handle(GetMemberStatusByIdQuery request, CancellationToken cancellationToken)
        {
            var result = new Result<MemberStatusDto>();
            var data = await _context.MemberStatuses.FirstOrDefaultAsync(q=>q.Id==request.Id, cancellationToken);
            if (data==null)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;

                result.Data = new MemberStatusDto
                {
                    Id = data.Id,
                    Name = data.Name
                };
            }

            return result;
        }
    }
}
