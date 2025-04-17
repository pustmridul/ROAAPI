using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;
using ResApp.Application.ROA.MunicipalityInfo.Models;
using ResApp.Application.ROA.Union.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResApp.Application.ROA.MunicipalityInfo.Queries
{
   
    public class GetWardByThanaQuery : IRequest<ListResult<WardDto>>
    {
        public int? ThanaId { get; set; }
        public int? MunicipalityId { get; set; }
        public int? UnionInfoId { get; set; }
        public string? AppId { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string? SearchText { get; set; }
        public string? Thana { get; set; }
    }

    public class GetWardByThanaQueryHandler : IRequestHandler<GetWardByThanaQuery, ListResult<WardDto>>
    {
        private readonly IMemDbContext _context;
        public GetWardByThanaQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<ListResult<WardDto>> Handle(GetWardByThanaQuery request, CancellationToken cancellationToken)
        {
            var result = new ListResult<WardDto>();
            //var data = await _context.Thanas.
            //    Where(x=> x.DistrictId== request.DistrictId).ToListAsync(cancellationToken);

            try
            {
                var query = _context.Wards
                             .Where(x => x.IsActive ==true) // Base filters
                             .AsQueryable(); // Start with IQueryable

                if (!string.IsNullOrEmpty(request.SearchText))
                {
                    string searchText = request.SearchText.ToLower();
                    query = query.Where(x => x.EnglishName!.ToLower().Contains(searchText));
                }

                if (!string.IsNullOrEmpty(request.Thana))
                {
                    string searchText = request.Thana.ToLower();
                    query = query.Where(x => x.Thana!.EnglishName!.ToLower().Contains(searchText));
                }

                if (request.ThanaId > 0)
                {
                   
                    query = query.Where(x => x.ThanaId == request.ThanaId);
                }

                if (request.MunicipalityId > 0)
                {

                    query = query.Where(x => x.MunicipalityId == request.MunicipalityId);
                }

                if (request.UnionInfoId > 0)
                {

                    query = query.Where(x => x.UnionInfoId == request.UnionInfoId);
                }
                //var data = await _context.Wards.Include(x => x.Thana).Include(x => x.Municipality).Include(x => x.UnionInfo)
                //                                    .Where(x => (request.ThanaId == 0 || x.ThanaId == request.ThanaId
                //                                     || request.MunicipalityId.GetValueOrDefault() == 0 || request.MunicipalityId == request.MunicipalityId) &&
                //                                        (!string.IsNullOrEmpty(request.SearchText) ? x.Thana!.EnglishName!.ToLower().Contains(request.SearchText.ToLower()) : true ||
                //                                        !string.IsNullOrEmpty(request.SearchText) ? x.Municipality!.EnglishName!.ToLower().Contains(request.SearchText!.ToLower()) : true)
                //                                        && x.IsActive == true).OrderBy(o => o.EnglishName)
                //                                        .ToPaginatedListAsync(request.PageNo, request.PageSize, cancellationToken);



                query = query.Include(x => x.Thana).Include(x => x.Municipality).Include(x => x.UnionInfo);
                // Apply ordering
                query = query.OrderBy(o => o.Id);

                // Apply pagination
                var data = await query.ToPaginatedListAsync(
                    request.PageNo,
                    request.PageSize,
                    cancellationToken
                );

                if (data.TotalCount == 0)
                {
                    result.HasError = true;
                    result.Messages.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
                    result.Count = data.TotalCount;
                    result.Data = data.Data.Select(s => new WardDto
                    {
                        Id = s.Id,
                        EnglishName = s.EnglishName,
                        BanglaName = s.BanglaName,
                        MunicipalityName = s.Municipality?.EnglishName,
                        ThanaName = s.Thana?.EnglishName,
                        UnionInfoName = s.UnionInfo?.EnglishName,
                        ThanaId = s.ThanaId.GetValueOrDefault(),
                        MunicipalityId = s.MunicipalityId.GetValueOrDefault(),
                        UnionInfoId = s.UnionInfoId.GetValueOrDefault(),
                    }).ToList();
                }

                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }
    }
}
