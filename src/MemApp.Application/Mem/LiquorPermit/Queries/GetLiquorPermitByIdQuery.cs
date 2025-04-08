using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Communication.Models;
using MemApp.Application.Mem.MiscItems.Models;
using MemApp.Application.Mem.MiscItems.Queries;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Communication.Queries
{
    public class GetLiquorPermitByIdQuery : IRequest<LiquorPermitModelReq>
    {
        public int MemberId { get; set; }
    }
    public class GetLiquorPermitByIdQueryHandler : IRequestHandler<GetLiquorPermitByIdQuery, LiquorPermitModelReq>
    {
        private readonly IMemDbContext _context;
        public GetLiquorPermitByIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<LiquorPermitModelReq> Handle(GetLiquorPermitByIdQuery request, CancellationToken cancellationToken)
        {
            var result = new LiquorPermitModelReq();
            var data = await _context.LiquorPermits.OrderByDescending(c=>c.Id).FirstOrDefaultAsync(q => q.MemberId == request.MemberId, cancellationToken);

            if (data != null)
            {
                result = new LiquorPermitModelReq
                {
                    Id = data.Id,
                    Title = data.Title,
                    MemberId = data.MemberId,
                    FileUrl = data.FileUrl
                };
            }

            return result;
        }
    }
}
