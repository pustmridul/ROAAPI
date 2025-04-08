using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Exceptions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Members.Models;
using MemApp.Domain.Entities.mem;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.MemberStatuss.Queries
{
    public class GetMemberFileByIdQuery : IRequest<MemberFileListVm>
    {
        public int Id { get; set; }
    }


    public class GetMemberFileByIdQueryHandler : IRequestHandler<GetMemberFileByIdQuery, MemberFileListVm>
    {
        private readonly IMemDbContext _context;
        public GetMemberFileByIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<MemberFileListVm> Handle(GetMemberFileByIdQuery request, CancellationToken cancellationToken)
        {
            var result = new MemberFileListVm();

            var data = await _context.MemFiles
                .Where(q => q.IsActive && q.MemberId == request.Id)
                .ToListAsync(cancellationToken);
              
            if (data==null)
            {
               // throw new NotFoundException();
            }
            else
            {
                result.DataCount= data.Count;
                result.DataList= data.Select(s=>new MemberFileVm
                {
                    FileData=s.FileContent,
                    Title=s.Titile
                }).ToList();
            }

            return result;
        }
    }
}
