using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using ResApp.Application.ROA.Union.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResApp.Application.ROA.MunicipalityInfo.Queries
{
    
    public class GetMuniByThanaIdQuery : IRequest<ListResult<UnionDto>>
    {
        public int ThanaId { get; set; }
        public string? AppId { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string? SearchText { get; set; }
     
    }

    public class GetUnionByThanaIdQueryHandler : IRequestHandler<GetMuniByThanaIdQuery, ListResult<UnionDto>>
    {
        private readonly IMemDbContext _context;
        public GetUnionByThanaIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<ListResult<UnionDto>> Handle(GetMuniByThanaIdQuery request, CancellationToken cancellationToken)
        {
            var result = new ListResult<UnionDto>();
          

            var data = await _context.Municipalities.Where(x => (request.ThanaId == 0 || x.ThanaId == request.ThanaId) &&
          (!string.IsNullOrEmpty(request.SearchText) ? x.EnglishName!.ToLower().Contains(request.SearchText.ToLower()) : true) && x.IsActive == true).OrderBy(o => o.EnglishName)
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
                result.Data = data.Data.Select(s => new UnionDto
                {
                    Id = s.Id,
                    EnglishName = s.EnglishName,
                    BanglaName = s.BanglaName,
                    ThanaId = s.ThanaId.GetValueOrDefault(),
                }).ToList();
            }

            return result;
        }
    }
}
