using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Members.Models;
using Microsoft.EntityFrameworkCore;
using ResApp.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.MemberTypes.Queries
{
    public class GetDistrictDetailsQuery : IRequest<Result<DistrictDto>>
    {
        public int DistrictId {  get; set; }
    }

    public class GetDistrictDetailsQueryHandler : IRequestHandler<GetDistrictDetailsQuery, Result<DistrictDto>>
    {
        private readonly IMemDbContext _context;
        public GetDistrictDetailsQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<Result<DistrictDto>> Handle(GetDistrictDetailsQuery request, CancellationToken cancellationToken)
        {
            var result = new Result<DistrictDto>();
            var data = await _context.Districts.Where(s=>s.Id==request.DistrictId).Include(s=>s.Division).FirstOrDefaultAsync(cancellationToken);

            if (data == null)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;
               // result.Count= data.Count;
                result.Data =new DistrictDto
                {
                    Id = data.Id,
                    EnglishName = data.EnglishName,
                    BanglaName = data.BanglaName,
                    DivisionId=data.DivisionId.GetValueOrDefault(),
                    DivisionName=data.Division!.EnglishName,
                };
            }

            return result;
        }
    }
}
