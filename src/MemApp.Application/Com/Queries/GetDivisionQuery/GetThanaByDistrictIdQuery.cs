using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;
using ResApp.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResApp.Application.Com.Queries.GetDivisionQuery
{
    public class GetThanaByDistrictIdQuery : IRequest<ListResult<ThanaDto>>
    {
        public int DistrictId {  get; set; }
        public string? AppId { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string? SearchText { get; set; }
    }

    public class GetThanaByDistrictIdQueryHandler : IRequestHandler<GetThanaByDistrictIdQuery, ListResult<ThanaDto>>
    {
        private readonly IMemDbContext _context;
        public GetThanaByDistrictIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<ListResult<ThanaDto>> Handle(GetThanaByDistrictIdQuery request, CancellationToken cancellationToken)
        {
            var result = new ListResult<ThanaDto>();
            //var data = await _context.Thanas.
            //    Where(x=> x.DistrictId== request.DistrictId).ToListAsync(cancellationToken);

            var data = await _context.Thanas.Include(x=>x.District).Include(x=>x.Zone).Where(x => (request.DistrictId == 0 || x.DistrictId == request.DistrictId) &&
          (!string.IsNullOrEmpty(request.SearchText) ? x.EnglishName!.ToLower().Contains(request.SearchText.ToLower()) : true)).OrderBy(o => o.DistrictId)
              .ToPaginatedListAsync(request.PageNo, request.PageSize, cancellationToken);

            if (data.TotalCount == 0)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;
                result.Count = data.TotalCount;
                result.Data = data.Data.Select(s => new ThanaDto
                {
                    Id = s.Id,
                    EnglishName = s.EnglishName,
                    BanglaName = s.BanglaName,
                    DistrictId=s.DistrictId.GetValueOrDefault(),
                    ZoneId=s.ZoneId.GetValueOrDefault(),
                    DistrictName=s.District?.EnglishName,
                    DistrictBanglaName=s.District?.BanglaName,
                    ZoneName=s.Zone?.EnglishName,
                    UnionCount = _context.UnionInfos.Count(u => u.ThanaId == s.Id)
                }).ToList();
            }

            return result;
        }
    }
}
