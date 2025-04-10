using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Committees.Models;
using Microsoft.EntityFrameworkCore;
using ResApp.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResApp.Application.ROA.CommitteeCategory.Queries
{
  
    public class GetAllCommitteeCatQuery : IRequest<ListResult<CommitteeCatReq>>
    {
        
    }

    public class GetAllCommitteeCatQueryHandler : IRequestHandler<GetAllCommitteeCatQuery, ListResult<CommitteeCatReq>>
    {
        private readonly IMemDbContext _context;
        public GetAllCommitteeCatQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<ListResult<CommitteeCatReq>> Handle(GetAllCommitteeCatQuery request, CancellationToken cancellationToken)
        {
            var result = new ListResult<CommitteeCatReq>();
            //var data = await _context.Thanas.
            //    Where(x=> x.DistrictId== request.DistrictId).ToListAsync(cancellationToken);
            var data = await _context.RoCommitteeCategories
                               .Where(q => q.IsActive)
                               .AsNoTracking()
                               .ToListAsync(cancellationToken);

            if (data.Count == 0)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;
                result.Count = data.Count;
                result.Data = data.Select(s => new CommitteeCatReq
                {
                    Id = s.Id,
                    Title = s.Title,                 
                }).ToList();
            }

            return result;
        }
    }
}
