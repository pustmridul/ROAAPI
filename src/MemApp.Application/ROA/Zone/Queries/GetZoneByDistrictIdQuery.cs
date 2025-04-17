using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;
using ResApp.Application.Models.DTOs;
using ResApp.Application.ROA.Zone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResApp.Application.ROA.Zone.Queries
{
    

    public class GetZoneByDistrictIdQuery : IRequest<ListResult<ZoneDto>>
    {
        public int DistrictId { get; set; }
        public string? AppId { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string? SearchText { get; set; }
    }

    public class GetZoneByDistrictIdQueryHandler : IRequestHandler<GetZoneByDistrictIdQuery, ListResult<ZoneDto>>
    {
        private readonly IMemDbContext _context;
        public GetZoneByDistrictIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<ListResult<ZoneDto>> Handle(GetZoneByDistrictIdQuery request, CancellationToken cancellationToken)
        {
            var result = new ListResult<ZoneDto>();
            //var data = await _context.Thanas.
            //    Where(x=> x.DistrictId== request.DistrictId).ToListAsync(cancellationToken);

            var data = await _context.ZoneInfos.Include(x=>x.District).Where(x => (request.DistrictId == 0 || x.DistrictId == request.DistrictId) &&
          (!string.IsNullOrEmpty(request.SearchText) ? x.EnglishName!.ToLower().Contains(request.SearchText.ToLower()) : true) && x.IsActive==true).OrderBy(o => o.EnglishName)
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
                result.Data = data.Data.Select(s => new ZoneDto
                {
                    Id = s.Id,
                    EnglishName = s.EnglishName,
                    BanglaName = s.BanglaName,
                    DistrictId = s.DistrictId.GetValueOrDefault(),
                    DistrictName=s.District!.EnglishName
                }).ToList();
            }

            return result;
        }
    }
}
