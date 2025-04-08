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

namespace MemApp.Application.Mem.MemberProfessions.Queries
{
    public class GetMemberProfessionByIdQuery : IRequest<Result<MemberProfessionDto>>
    {
        public int Id { get; set; }
    }


    public class GetMemberProfessionByIdQueryHandler : IRequestHandler<GetMemberProfessionByIdQuery, Result<MemberProfessionDto>>
    {
        private readonly IMemDbContext _context;
        public GetMemberProfessionByIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<Result<MemberProfessionDto>> Handle(GetMemberProfessionByIdQuery request, CancellationToken cancellationToken)
        {
            var result = new Result<MemberProfessionDto>();
            var data = await _context.MemberProfessions.FirstOrDefaultAsync(q=>q.Id==request.Id, cancellationToken);
            if (data==null)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;

                result.Data = new MemberProfessionDto
                {
                    Id = data.Id,
                    Name = data.Name
                };
            }

            return result;
        }
    }
}
