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
  
    public class GetCommitteeCatByIdQuery : IRequest<Result<CommitteeCatReq>>
    {
        public int CategoryId { get; set; }
    }

    public class GetCommitteeCatByIdQueryHandler : IRequestHandler<GetCommitteeCatByIdQuery, Result<CommitteeCatReq>>
    {
        private readonly IMemDbContext _context;
        public GetCommitteeCatByIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<Result<CommitteeCatReq>> Handle(GetCommitteeCatByIdQuery request, CancellationToken cancellationToken)
        {
            var result = new Result<CommitteeCatReq>();
            var data = await _context.RoCommitteeCategories.Where(s => s.Id == request.CategoryId).FirstOrDefaultAsync(cancellationToken);

            if (data == null)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;
                // result.Count= data.Count;
                result.Data = new CommitteeCatReq
                {
                    Id = data.Id,
                    Title = data.Title,
                   
                };
            }

            return result;
        }
    }
}
