using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Committees.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResApp.Application.ROA.MemberCategory.Queries
{
  

    public class GetAllRoMemberCatQuery : IRequest<ListResult<RoMemberCategoryDto>>
    {

    }

    public class GetAllRoMemberCatQueryHandler : IRequestHandler<GetAllRoMemberCatQuery, ListResult<RoMemberCategoryDto>>
    {
        private readonly IMemDbContext _context;
        public GetAllRoMemberCatQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<ListResult<RoMemberCategoryDto>> Handle(GetAllRoMemberCatQuery request, CancellationToken cancellationToken)
        {
            var result = new ListResult<RoMemberCategoryDto>();
            //var data = await _context.Thanas.
            //    Where(x=> x.DistrictId== request.DistrictId).ToListAsync(cancellationToken);
            var data = await _context.RoaMemberCategories
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
                result.Data = data.Select(s => new RoMemberCategoryDto
                {
                    Id = s.Id,
                    SeatQuantity = s.SeatQuantity,
                    Description = s.Description,
                }).ToList();
            }

            return result;
        }
    }
}
