using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;
using Res.Domain.Entities;
using ResApp.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResApp.Application.Com.Queries.GetDivisionQuery
{
    public class GetDistrictDivisionIdQuery : IRequest<ListResult<DistrictDto>>
    {
        public int DivId {  get; set; }
        public string? AppId { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string? SearchText { get; set; }
    }

    public class GetDistrictDivisionIdQueryHandler : IRequestHandler<GetDistrictDivisionIdQuery, ListResult<DistrictDto>>
    {
        private readonly IMemDbContext _context;
        public GetDistrictDivisionIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<ListResult<DistrictDto>> Handle(GetDistrictDivisionIdQuery request, CancellationToken cancellationToken)
        {
            var result = new ListResult<DistrictDto>();
            //var data = await _context.Districts.
            //    Where(x=> x.DivisionId== request.DivId).ToListAsync(cancellationToken);
            /*q => q.IsActive && q.AppId == request.AppId*/

           // var data2 = PaginatedResult<District>(request.PageNo, request.PageSize,cancellationToken);
            var data = await _context.Districts.Where( x=> (request.DivId == 0 || x.DivisionId == request.DivId) &&
            (!string.IsNullOrEmpty(request.SearchText) ? x.EnglishName!.ToLower().Contains(request.SearchText.ToLower()) : true)).OrderBy(o => o.EnglishName)
                .ToPaginatedListAsync(request.PageNo, request.PageSize, cancellationToken);

            var dataCount = await _context.Districts.
                Where(x=> x.DivisionId== request.DivId).CountAsync(cancellationToken);
            //var dataCount = await _context.Districts.Where(q => q.IsActive && q.AppId == request.AppId
            //&& (!string.IsNullOrEmpty(request.SearchText) ? q.UserName.ToLower().Contains(request.SearchText.ToLower()) : true)).CountAsync(cancellationToken);

            if (data.TotalCount == 0)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;
                result.Count = data.TotalCount;
                result.Data = data.Data.Select(s => new DistrictDto
                {
                    Id = s.Id,
                    EnglishName = s.EnglishName,
                    BanglaName = s.BanglaName,
                    DivisionId=s.DivisionId.GetValueOrDefault(),
                }).ToList();
            }

            return result;
        }
    }
}
